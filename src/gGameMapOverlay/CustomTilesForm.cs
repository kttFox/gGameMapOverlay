using gGameMapOverlay.Overlay;

namespace gGameMapOverlay;

/// <summary>
/// 自分で描くマス (カスタム) の編集画面。グループごとに、マス・色・不透明度・表示するかを決める。
/// マスはキャラクターのいるマスからの相対位置で、オーバーレイではプレイヤーの枠と同じく画面に固定して描く。
/// グループの写し (Groups) を編集し、OK で閉じたときだけ呼び出し側が反映する (モーダルで表示する)。
/// </summary>
internal sealed partial class CustomTilesForm : Form {
	// 追加したグループに順に使う色 (マップのデータの種類・プレイヤーの枠と見分けやすい色)。
	private static readonly Color[] Palette = [
		CustomTileGroup.DefaultColor,
		Color.FromArgb( 255, 152, 0 ),
		Color.FromArgb( 233, 30, 99 ),
		Color.FromArgb( 205, 220, 57 ),
		Color.FromArgb( 255, 255, 255 ),
		Color.FromArgb( 121, 134, 203 ),
	];

	private readonly AppConfig config;
	// 編集中のグループ (config のものの写し)。
	private readonly List<CustomTileGroup> groups;
	// 編集中のグループをオーバーレイに描かせる (閉じたら null で設定のグループに戻す)。
	private readonly Action<IReadOnlyList<CustomTileGroup>?> preview;
	// 元に戻す・やり直すための、変える前のグループの写し。
	private readonly Stack<List<CustomTileGroup>> undoStack = new();
	private readonly Stack<List<CustomTileGroup>> redoStack = new();
	// 続けて同じものを変えるとき (名前の入力など) は 1 回にまとめる。その変更の種類。
	private string? lastEditKey;
	// ドラッグを始めたときの写し。実際にマスが変わったら undoStack に積む。
	private List<CustomTileGroup>? strokeSnapshot;
	// 背景に映すゲーム画面を撮る (画像と、その画面でのマスの幅)。ゲームが見つからなければ null。
	private readonly Func<Task<(Bitmap Image, double TileWidth)?>> captureGame;
	private bool refreshing;
	// 画面を表示し終えたか。
	private bool shown;
	// 背景に映すゲーム画面のメッセージ (撮れなかった理由など)。
	private string gameStatus = "";

	public CustomTilesForm( AppConfig config, Action<IReadOnlyList<CustomTileGroup>?> preview, Func<Task<(Bitmap Image, double TileWidth)?>> captureGame ) {
		this.config = config;
		this.preview = preview;
		groups = config.CustomGroups.Select( group => group.Clone() ).ToList();
		this.captureGame = captureGame;
		InitializeComponent();
		canvas.Groups = groups;
		canvas.PlayerColor = Color.FromArgb( 255, config.GetPlayerColor() );
		canvas.RequestGroup = () => AddGroup();
		AddShapeButtons();
		refreshing = true;
		for( var thickness = CustomTileShape.MinThickness; thickness <= CustomTileShape.MaxThickness; thickness++ ) {
			thicknessBox.Items.Add( $"1/{thickness} マス" );
		}
		thicknessBox.SelectedIndex = CustomTileShape.DefaultThickness - CustomTileShape.MinThickness;
		gameCheck.Checked = config.CustomEditorShowGame;
		refreshing = false;
		recaptureButton.Enabled = gameCheck.Checked;
		RefreshList( groups.Count > 0 ? 0 : -1 );
	}

	/// <summary>編集したグループ。OK で閉じたときに config に入れる。</summary>
	public List<CustomTileGroup> Groups => groups;

	private CustomTileGroup? Selected =>
		groupList.SelectedIndices.Count > 0 && groupList.SelectedIndices[0] < groups.Count ? groups[groupList.SelectedIndices[0]] : null;

	/// <summary>描く形を選ぶボタン (形の見本の絵) を並べる。</summary>
	private void AddShapeButtons() {
		var size = new Size( LogicalToDeviceUnits( 40 ), LogicalToDeviceUnits( 28 ) );
		foreach( var shape in CustomTileShape.All ) {
			var button = new RadioButton {
				Appearance = Appearance.Button,
				Size = size,
				Margin = new Padding( 1 ),
				Image = ShapeImage( shape, size, canvas.Thickness ),
				ImageAlign = ContentAlignment.MiddleCenter,
				Checked = shape == CustomTileShape.Full,
				Tag = shape,
				UseVisualStyleBackColor = true,
			};
			button.CheckedChanged += ( _, _ ) => {
				if( button.Checked ) {
					canvas.Shape = shape;
				}
			};
			shapeTip.SetToolTip( button, shape.Name );
			shapePanel.Controls.Add( button );
		}
	}

	/// <summary>線の太さを変えたとき、形の見本の絵をその太さで描き直す。</summary>
	private void RefreshShapeImages() {
		foreach( var button in shapePanel.Controls.OfType<RadioButton>() ) {
			var old = button.Image;
			button.Image = ShapeImage( (CustomTileShape)button.Tag!, button.Size, canvas.Thickness );
			old?.Dispose();
		}
	}

	/// <summary>形の見本の絵 (マスの菱形に、形を太さ 1/thickness マスで描く)。</summary>
	private static Bitmap ShapeImage( CustomTileShape shape, Size button, int thickness ) {
		var width = button.Width - 8;
		var bitmap = new Bitmap( width, width / 2 + 2 );
		using var graphics = Graphics.FromImage( bitmap );
		graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		var grid = new IsoGrid( new PointF( bitmap.Width / 2f, bitmap.Height / 2f ), new Parsing.GameCoordinate( 0, 0 ), width - 2, ( width - 2 ) / 2.0 );
		var color = Color.FromArgb( 255, 0, 120, 215 );
		if( shape.Mask == 0 ) {
			// 小さいマスずつ: 小さいマスの格子 (3 等分) と、塗った小さいマス 1 つ。
			var sub = CustomTileGroup.SubGrid( grid );
			using var line = new Pen( Color.Gray );
			for( var i = 1; i < CustomTileGroup.Division; i++ ) {
				graphics.DrawLine( line, sub.GridPoint( i, 0 ), sub.GridPoint( i, CustomTileGroup.Division ) );
				graphics.DrawLine( line, sub.GridPoint( 0, i ), sub.GridPoint( CustomTileGroup.Division, i ) );
			}
		}
		var mask = shape.Mask == 0 ? CustomTileShape.PartMask( 2, 0 ) : shape.Mask;
		var group = new CustomTileGroup { Cells = [[0, 0, mask, thickness]] };
		var tiles = group.ToCustomTiles() with { Color = color };
		OverlayForm.DrawCustomTiles( graphics, grid, tiles, new RectangleF( 0, 0, bitmap.Width, bitmap.Height ) );
		using var outline = new Pen( Color.DarkGray );
		graphics.DrawPolygon( outline, grid.Tile( 0, 0 ) );
		return bitmap;
	}

	/// <summary>グループの一覧を作り直して、select 番目を選ぶ (-1 なら選ばない)。</summary>
	private void RefreshList( int select ) {
		refreshing = true;
		try {
			groupList.BeginUpdate();
			groupList.Items.Clear();
			swatches.Images.Clear();
			foreach( var group in groups ) {
				swatches.Images.Add( Swatch( group ) );
				groupList.Items.Add( new ListViewItem( group.Name, swatches.Images.Count - 1 ) { Checked = group.Shown } );
			}
			if( select >= 0 && select < groupList.Items.Count ) {
				groupList.Items[select].Selected = true;
				groupList.Items[select].Focused = true;
				groupList.EnsureVisible( select );
			}
			groupList.EndUpdate();
		} finally {
			refreshing = false;
		}
		ShowSelected();
	}

	/// <summary>一覧に出す色の見本 (不透明で見せる)。</summary>
	private Bitmap Swatch( CustomTileGroup group ) {
		var size = swatches.ImageSize;
		var bitmap = new Bitmap( size.Width, size.Height );
		using var graphics = Graphics.FromImage( bitmap );
		using var fill = new SolidBrush( Color.FromArgb( 255, group.GetColor() ) );
		graphics.FillRectangle( fill, 0, 0, size.Width - 1, size.Height - 1 );
		graphics.DrawRectangle( Pens.Gray, 0, 0, size.Width - 1, size.Height - 1 );
		return bitmap;
	}

	/// <summary>選んでいるグループの名前・色・不透明度を表示する。</summary>
	private void ShowSelected() {
		var group = Selected;
		refreshing = true;
		try {
			foreach( var control in new Control[] { nameBox, colorButton, opacityCheck, removeButton } ) {
				control.Enabled = group is not null;
			}
			upButton.Enabled = group is not null && groups.IndexOf( group ) > 0;
			downButton.Enabled = group is not null && groups.IndexOf( group ) < groups.Count - 1;
			nameBox.Text = group?.Name ?? "";
			colorButton.BackColor = group is null ? SystemColors.Control : Color.FromArgb( 255, group.GetColor() );
			canvas.OverallOpacity = config.OverlayOpacity;
			opacityCheck.Checked = group?.OwnOpacity ?? false;
			opacityBox.Enabled = group is { OwnOpacity: true }; // 個別でなければ全体の不透明度を (変えられない状態で) 表示する
			opacityBox.Value = Math.Clamp( group?.GetOpacity( config.OverlayOpacity ) ?? AppConfig.DefaultOverlayOpacity, opacityBox.Minimum, opacityBox.Maximum );
		} finally {
			refreshing = false;
		}
		canvas.Selected = group;
		canvas.Invalidate();
		ShowStatus();
	}

	private void ShowStatus() {
		var hover = canvas.HoverCell is { } cell ? $"マウスの位置: キャラクターから X {cell.X:+0;-0;0}, Y {cell.Y:+0;-0;0}　" : "";
		var count = Selected is { } group ? $"「{group.Name}」 {group.Cells.Count} マス" : "グループなし";
		statusLabel.Text = hover + count + gameStatus;
	}

	private void Canvas_ZoomChanged( object? sender, EventArgs e ) => zoomResetButton.Text = $"{canvas.ZoomPercent}%";

	private void ZoomOutButton_Click( object? sender, EventArgs e ) => canvas.Zoom( -1 );

	private void ZoomResetButton_Click( object? sender, EventArgs e ) => canvas.ResetZoom();

	private void ZoomInButton_Click( object? sender, EventArgs e ) => canvas.Zoom( 1 );

	private List<CustomTileGroup> Snapshot() => groups.Select( group => group.Clone() ).ToList();

	/// <summary>これから変える前の状態を、元に戻せるように覚える。key が前回と同じなら前回とまとめる。</summary>
	private void RecordUndo( string? key = null ) {
		if( key is not null && key == lastEditKey ) {
			return;
		}
		lastEditKey = key;
		undoStack.Push( Snapshot() );
		redoStack.Clear();
		RefreshUndoButtons();
	}

	private void RefreshUndoButtons() {
		undoButton.Enabled = undoStack.Count > 0;
		redoButton.Enabled = redoStack.Count > 0;
	}

	/// <summary>from の直前の状態に戻し、今の状態を to に積む (元に戻す・やり直す)。</summary>
	private void Restore( Stack<List<CustomTileGroup>> from, Stack<List<CustomTileGroup>> to ) {
		if( from.Count == 0 ) {
			return;
		}
		var index = Selected is { } group ? groups.IndexOf( group ) : 0;
		to.Push( Snapshot() );
		groups.Clear();
		groups.AddRange( from.Pop() );
		lastEditKey = null;
		RefreshList( Math.Min( index, groups.Count - 1 ) );
		ShowPreview();
		RefreshUndoButtons();
	}

	protected override bool ProcessCmdKey( ref Message msg, Keys keyData ) {
		// 名前の入力中は、テキストボックス自身の元に戻すを使う。
		if( ActiveControl is not TextBox ) {
			// 格子の上でスペースを押すのは動かすため。フォーカスのあるボタンを押さないようにする。
			if( keyData == Keys.Space && canvas.ClientRectangle.Contains( canvas.PointToClient( MousePosition ) ) ) {
				return true;
			}
			switch( keyData ) {
				case Keys.Control | Keys.Z:
					Restore( undoStack, redoStack );
					return true;
				case Keys.Control | Keys.Y:
				case Keys.Control | Keys.Shift | Keys.Z:
					Restore( redoStack, undoStack );
					return true;
			}
		}
		return base.ProcessCmdKey( ref msg, keyData );
	}

	/// <summary>編集中のグループをオーバーレイに描かせる。</summary>
	private void ShowPreview() => preview( groups );

	protected override void OnShown( EventArgs e ) {
		base.OnShown( e );
		shown = true;
		ShowPreview();
		if( gameCheck.Checked ) {
			_ = CaptureGameAsync();
		}
	}

	protected override void OnFormClosed( FormClosedEventArgs e ) {
		base.OnFormClosed( e );
		preview( null );
		canvas.SetGameImage( null, 0 );
	}

	/// <summary>
	/// 背景に映すゲーム画面を撮る。この画面がゲームに重なっていると写り込むので、撮る間だけ透明にする
	/// (オーバーレイは captureGame が隠す)。
	/// </summary>
	private async Task CaptureGameAsync() {
		recaptureButton.Enabled = false;
		var opacity = Opacity;
		Opacity = 0;
		(Bitmap Image, double TileWidth)? shot = null;
		try {
			shot = await captureGame(); // 透明にした画面が描き直されるのも、ここで待つ
		} finally {
			Opacity = opacity;
			recaptureButton.Enabled = gameCheck.Checked;
		}
		if( IsDisposed || !gameCheck.Checked ) {
			shot?.Image.Dispose();
			return;
		}
		gameStatus = shot is null ? "　(ゲームの画面が見つかりません)" : "";
		canvas.SetGameImage( shot?.Image, shot?.TileWidth ?? 0 );
		ShowStatus();
	}

	/// <summary>グループを末尾に足して選ぶ。</summary>
	private CustomTileGroup AddGroup() {
		var number = 1;
		while( groups.Any( group => group.Name == $"グループ {number}" ) ) {
			number++;
		}
		var added = new CustomTileGroup { Name = $"グループ {number}" };
		added.SetColor( Palette[groups.Count % Palette.Length] );
		RecordUndo();
		groups.Add( added );
		RefreshList( groups.Count - 1 );
		ShowPreview();
		return added;
	}

	private void MoveGroup( int step ) {
		if( Selected is not { } group ) {
			return;
		}
		var index = groups.IndexOf( group );
		var target = index + step;
		if( target < 0 || target >= groups.Count ) {
			return;
		}
		RecordUndo();
		( groups[index], groups[target] ) = ( groups[target], groups[index] );
		RefreshList( target );
		ShowPreview();
	}

	// ---- イベントハンドラ (デザイナーから接続) ------------------------------------------

	private void GroupList_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			ShowSelected();
		}
	}

	private void GroupList_ItemChecked( object? sender, ItemCheckedEventArgs e ) {
		// ListView は表示するときにも (チェックを変えていなくても、いったん外した状態で) 通知するので、表示した後に変わったときだけ扱う。
		if( refreshing || !shown || e.Item.Index >= groups.Count || groups[e.Item.Index].Shown == e.Item.Checked ) {
			return;
		}
		RecordUndo();
		groups[e.Item.Index].Shown = e.Item.Checked;
		canvas.Invalidate();
		ShowPreview();
	}

	private void GroupList_Resize( object? sender, EventArgs e ) =>
		groupColumn.Width = Math.Max( 40, groupList.ClientSize.Width - 4 );

	private void AddButton_Click( object? sender, EventArgs e ) => AddGroup();

	private void RemoveButton_Click( object? sender, EventArgs e ) {
		if( Selected is not { } group ) {
			return;
		}
		if( group.Cells.Count > 0 && MessageBox.Show( this, $"「{group.Name}」を削除しますか? ({group.Cells.Count} マス)", Text,
			MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) != DialogResult.OK ) {
			return;
		}
		var index = groups.IndexOf( group );
		RecordUndo();
		groups.RemoveAt( index );
		RefreshList( Math.Min( index, groups.Count - 1 ) );
		ShowPreview();
	}

	private void UpButton_Click( object? sender, EventArgs e ) => MoveGroup( -1 );

	private void DownButton_Click( object? sender, EventArgs e ) => MoveGroup( 1 );

	private void NameBox_TextChanged( object? sender, EventArgs e ) {
		if( refreshing || Selected is not { } group || group.Name == nameBox.Text ) {
			return;
		}
		RecordUndo( $"name:{groups.IndexOf( group )}" );
		group.Name = nameBox.Text;
		groupList.Items[groups.IndexOf( group )].Text = group.Name;
		ShowStatus();
	}

	private void ColorButton_Click( object? sender, EventArgs e ) {
		if( Selected is not { } group ) {
			return;
		}
		using var dialog = new ColorDialog { Color = Color.FromArgb( 255, group.GetColor() ), FullOpen = true };
		if( dialog.ShowDialog( this ) != DialogResult.OK ) {
			return;
		}
		RecordUndo();
		group.SetColor( dialog.Color );
		RefreshList( groups.IndexOf( group ) );
		ShowPreview();
	}

	private void OpacityBox_ValueChanged( object? sender, EventArgs e ) {
		if( refreshing || Selected is not { } group
			|| ( group.OwnOpacity == opacityCheck.Checked && ( !group.OwnOpacity || group.Opacity == (int)opacityBox.Value ) ) ) {
			return;
		}
		RecordUndo( $"opacity:{groups.IndexOf( group )}" );
		group.OwnOpacity = opacityCheck.Checked;
		if( group.OwnOpacity ) {
			group.Opacity = (int)opacityBox.Value; // チェックを入れたときは今の値 (全体の値) から始める
		}
		RefreshList( groups.IndexOf( group ) ); // 個別をやめたら全体の値を表示する
		canvas.Invalidate();
		ShowPreview();
	}

	private void ThicknessBox_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( thicknessBox.SelectedIndex < 0 ) {
			return;
		}
		canvas.Thickness = CustomTileShape.MinThickness + thicknessBox.SelectedIndex;
		RefreshShapeImages();
	}

	private void Canvas_StrokeStarting( object? sender, EventArgs e ) => strokeSnapshot = Snapshot();

	private void Canvas_CellsChanged( object? sender, EventArgs e ) {
		if( strokeSnapshot is not null ) { // ドラッグ 1 回を 1 回の変更として戻せるように
			undoStack.Push( strokeSnapshot );
			redoStack.Clear();
			strokeSnapshot = null;
			lastEditKey = null;
			RefreshUndoButtons();
		}
		ShowStatus();
		ShowPreview();
	}

	private void Canvas_HoverChanged( object? sender, EventArgs e ) => ShowStatus();

	private void GameCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( refreshing ) {
			return;
		}
		config.CustomEditorShowGame = gameCheck.Checked;
		recaptureButton.Enabled = gameCheck.Checked;
		if( gameCheck.Checked ) {
			_ = CaptureGameAsync();
		} else {
			gameStatus = "";
			canvas.SetGameImage( null, 0 );
			ShowStatus();
		}
	}

	private void UndoButton_Click( object? sender, EventArgs e ) => Restore( undoStack, redoStack );

	private void RedoButton_Click( object? sender, EventArgs e ) => Restore( redoStack, undoStack );

	private void RecaptureButton_Click( object? sender, EventArgs e ) => _ = CaptureGameAsync();
}
