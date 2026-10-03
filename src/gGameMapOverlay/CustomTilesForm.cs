using gGameMapOverlay.Overlay;

namespace gGameMapOverlay;

/// <summary>
/// 自分で描くマス (カスタム) の編集画面。グループごとに、不透明度・表示するかと、レイヤー (色ごとのマス) を決める。
/// マスはキャラクターのいるマスからの相対位置で、オーバーレイではプレイヤーの枠と同じく画面に固定して描く。
/// グループの写し (Groups) を編集し、OK で閉じたときだけ呼び出し側が反映する (モーダルで表示する)。
/// </summary>
internal sealed partial class CustomTilesForm : Form {
	// 追加したグループ・レイヤーに順に使う色 (マップのデータの種類・プレイヤーの枠と見分けやすい色)。
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
		canvas.RequestLayer = () => AddGroup().Layers[0];
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

	private int SelectedLayerIndex => layerList.SelectedIndices.Count > 0 ? layerList.SelectedIndices[0] : -1;

	/// <summary>選んでいるレイヤー (描く・消す対象)。</summary>
	private CustomTileLayer? SelectedLayer =>
		Selected is { } group && SelectedLayerIndex is var index && index >= 0 && index < group.Layers.Count ? group.Layers[index] : null;

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
		var layer = new CustomTileLayer { Cells = [[0, 0, mask, thickness]] };
		var tiles = layer.ToCustomTiles() with { Color = color };
		OverlayForm.DrawCustomTiles( graphics, grid, tiles, new RectangleF( 0, 0, bitmap.Width, bitmap.Height ) );
		using var outline = new Pen( Color.DarkGray );
		graphics.DrawPolygon( outline, grid.Tile( 0, 0 ) );
		return bitmap;
	}

	/// <summary>グループの一覧を作り直して、select 番目を選ぶ (-1 なら選ばない)。そのグループのレイヤーは layer 番目を選ぶ。</summary>
	private void RefreshList( int select, int layer = 0 ) {
		refreshing = true;
		try {
			groupList.BeginUpdate();
			groupList.Items.Clear();
			swatches.Images.Clear();
			foreach( var group in groups ) {
				swatches.Images.Add( Swatch( group, swatches.ImageSize ) );
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
		ShowSelected( layer );
	}

	/// <summary>グループの色の見本 (レイヤーの色を左から順に縦の帯で並べる。不透明で見せる)。</summary>
	internal static Bitmap Swatch( CustomTileGroup group, Size size ) {
		var bitmap = new Bitmap( Math.Max( 1, size.Width ), Math.Max( 1, size.Height ) );
		using var graphics = Graphics.FromImage( bitmap );
		var colors = group.Layers.Select( layer => Color.FromArgb( 255, layer.GetColor() ) ).DefaultIfEmpty( Color.FromArgb( 255, CustomTileGroup.DefaultColor ) ).ToList();
		for( var i = 0; i < colors.Count; i++ ) {
			var left = ( size.Width - 1 ) * i / colors.Count;
			var right = ( size.Width - 1 ) * ( i + 1 ) / colors.Count;
			using var fill = new SolidBrush( colors[i] );
			graphics.FillRectangle( fill, left, 0, right - left, size.Height - 1 );
		}
		graphics.DrawRectangle( Pens.Gray, 0, 0, size.Width - 1, size.Height - 1 );
		return bitmap;
	}

	/// <summary>レイヤーの色の見本 (不透明で見せる)。</summary>
	private static Bitmap LayerSwatch( CustomTileLayer layer, Size size ) {
		var bitmap = new Bitmap( size.Width, size.Height );
		using var graphics = Graphics.FromImage( bitmap );
		using var fill = new SolidBrush( Color.FromArgb( 255, layer.GetColor() ) );
		graphics.FillRectangle( fill, 0, 0, size.Width - 1, size.Height - 1 );
		graphics.DrawRectangle( Pens.Gray, 0, 0, size.Width - 1, size.Height - 1 );
		return bitmap;
	}

	/// <summary>選んでいるグループのレイヤーの一覧を作り直して、select 番目を選ぶ。</summary>
	private void RefreshLayers( int select ) {
		var group = Selected;
		var wasRefreshing = refreshing;
		refreshing = true;
		try {
			layerList.BeginUpdate();
			layerList.Items.Clear();
			layerSwatches.Images.Clear();
			for( var i = 0; i < ( group?.Layers.Count ?? 0 ); i++ ) {
				var layer = group!.Layers[i];
				layerSwatches.Images.Add( LayerSwatch( layer, layerSwatches.ImageSize ) );
				layerList.Items.Add( new ListViewItem( $"レイヤー {i + 1} ({layer.Cells.Count} マス)", i ) );
			}
			select = Math.Min( Math.Max( select, 0 ), layerList.Items.Count - 1 );
			if( select >= 0 ) {
				layerList.Items[select].Selected = true;
				layerList.Items[select].Focused = true;
				layerList.EnsureVisible( select );
			}
			layerList.EndUpdate();
		} finally {
			refreshing = wasRefreshing;
		}
		ShowLayer();
	}

	/// <summary>選んでいるレイヤーの色を表示し、描く対象にする。</summary>
	private void ShowLayer() {
		var group = Selected;
		var layer = SelectedLayer;
		var index = SelectedLayerIndex;
		colorButton.Enabled = layer is not null;
		colorButton.BackColor = layer is null ? SystemColors.Control : Color.FromArgb( 255, layer.GetColor() );
		layerAddButton.Enabled = group is not null;
		layerRemoveButton.Enabled = group is { Layers.Count: > 1 } && layer is not null;
		layerUpButton.Enabled = layer is not null && index > 0;
		layerDownButton.Enabled = layer is not null && index < group!.Layers.Count - 1;
		canvas.Selected = layer;
		canvas.Invalidate();
	}

	/// <summary>レイヤーの一覧の、マスの数の表示を直す。</summary>
	private void RefreshLayerCounts() {
		if( Selected is not { } group ) {
			return;
		}
		for( var i = 0; i < group.Layers.Count && i < layerList.Items.Count; i++ ) {
			layerList.Items[i].Text = $"レイヤー {i + 1} ({group.Layers[i].Cells.Count} マス)";
		}
	}

	/// <summary>選んでいるグループの名前・不透明度・レイヤーを表示する。レイヤーは layer 番目を選ぶ。</summary>
	private void ShowSelected( int layer = 0 ) {
		var group = Selected;
		refreshing = true;
		try {
			foreach( var control in new Control[] { nameBox, opacityCheck, removeButton } ) {
				control.Enabled = group is not null;
			}
			upButton.Enabled = group is not null && groups.IndexOf( group ) > 0;
			downButton.Enabled = group is not null && groups.IndexOf( group ) < groups.Count - 1;
			nameBox.Text = group?.Name ?? "";
			canvas.OverallOpacity = config.OverlayOpacity;
			opacityCheck.Checked = group?.OwnOpacity ?? false;
			opacityBox.Enabled = group is { OwnOpacity: true }; // 個別でなければ全体の不透明度を (変えられない状態で) 表示する
			opacityBox.Value = Math.Clamp( group?.GetOpacity( config.OverlayOpacity ) ?? AppConfig.DefaultOverlayOpacity, opacityBox.Minimum, opacityBox.Maximum );
		} finally {
			refreshing = false;
		}
		canvas.SelectedGroup = group;
		RefreshLayers( layer );
		ShowStatus();
	}

	private void ShowStatus() {
		var hover = canvas.HoverCell is { } cell ? $"マウスの位置: キャラクターから X {cell.X:+0;-0;0}, Y {cell.Y:+0;-0;0}　" : "";
		var count = Selected is { } group ? $"「{group.Name}」 {group.CellCount} マス" : "グループなし";
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
		var layer = Math.Max( 0, SelectedLayerIndex );
		to.Push( Snapshot() );
		groups.Clear();
		groups.AddRange( from.Pop() );
		lastEditKey = null;
		RefreshList( Math.Min( index, groups.Count - 1 ), layer );
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
		var layer = new CustomTileLayer();
		layer.SetColor( Palette[groups.Count % Palette.Length] );
		var added = new CustomTileGroup { Name = $"グループ {number}", Layers = [layer] };
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
		if( group.CellCount > 0 && MessageBox.Show( this, $"「{group.Name}」を削除しますか? ({group.CellCount} マス)", Text,
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
		if( Selected is not { } group || SelectedLayer is not { } layer ) {
			return;
		}
		using var dialog = new ColorDialog { Color = Color.FromArgb( 255, layer.GetColor() ), FullOpen = true };
		if( dialog.ShowDialog( this ) != DialogResult.OK ) {
			return;
		}
		RecordUndo();
		layer.SetColor( dialog.Color );
		RefreshList( groups.IndexOf( group ), group.Layers.IndexOf( layer ) );
		ShowPreview();
	}

	private void LayerList_SelectedIndexChanged( object? sender, EventArgs e ) {
		// 選び直すときは、いったん何も選んでいない状態を通るので、選んだときだけ扱う。
		if( !refreshing && layerList.SelectedIndices.Count > 0 ) {
			ShowLayer();
		}
	}

	private void LayerList_Resize( object? sender, EventArgs e ) =>
		layerColumn.Width = Math.Max( 40, layerList.ClientSize.Width - 4 );

	// スプリッターを掴んだ位置 (スプリッターの上端からのずれ)。掴んでいなければ null。
	// 標準のドラッグは離すまで高さが変わらないので、IsSplitterFixed で止めて自分で動かす。
	private int? splitGrab;

	private void ListSplit_MouseDown( object? sender, MouseEventArgs e ) {
		if( e.Button == MouseButtons.Left && listSplit.SplitterRectangle.Contains( e.Location ) ) {
			splitGrab = e.Y - listSplit.SplitterDistance;
		}
	}

	private void ListSplit_MouseMove( object? sender, MouseEventArgs e ) {
		if( splitGrab is not { } grab ) {
			// パネルは Cursor を親から受け継ぐので、デザイナーでパネルの Cursor を Default にしてある。
			listSplit.Cursor = listSplit.SplitterRectangle.Contains( e.Location ) ? Cursors.HSplit : Cursors.Default;
			return;
		}
		var max = listSplit.Height - listSplit.Panel2MinSize - listSplit.SplitterWidth;
		var distance = Math.Max( listSplit.Panel1MinSize, Math.Min( max, e.Y - grab ) );
		if( distance != listSplit.SplitterDistance ) {
			listSplit.SplitterDistance = distance;
			listSplit.Invalidate(); // 新しい位置に横線を描き直す (位置を変えただけでは描き直されない)
			listSplit.Update(); // マウスに遅れないよう、すぐ描き直す
		}
	}

	private void ListSplit_MouseUp( object? sender, MouseEventArgs e ) => splitGrab = null;

	private void ListSplit_MouseLeave( object? sender, EventArgs e ) {
		if( splitGrab is null ) {
			listSplit.Cursor = Cursors.Default;
		}
	}

	// 大きさが変わったら横線を描き直す (SplitContainer は変わった部分しか描き直さない)。
	private void ListSplit_Resize( object? sender, EventArgs e ) => listSplit.Invalidate();

	/// <summary>グループとレイヤーの間のスプリッターに、動かせるのが分かるよう横線を描く。</summary>
	private void ListSplit_Paint( object? sender, PaintEventArgs e ) {
		var bar = listSplit.SplitterRectangle;
		var y = bar.Top + bar.Height / 2;
		var inset = bar.Width / 12;
		e.Graphics.DrawLine( SystemPens.ControlDark, bar.Left + inset, y, bar.Right - 1 - inset, y );
		e.Graphics.DrawLine( SystemPens.ControlLightLight, bar.Left + inset, y + 1, bar.Right - 1 - inset, y + 1 );
	}

	/// <summary>選んでいるグループに、まだ使っていない色のレイヤーを足して選ぶ。</summary>
	private void LayerAddButton_Click( object? sender, EventArgs e ) {
		if( Selected is not { } group ) {
			return;
		}
		var used = group.Layers.Select( layer => layer.GetColor().ToArgb() & 0xFFFFFF ).ToHashSet();
		var start = groups.IndexOf( group ) + group.Layers.Count;
		var color = Enumerable.Range( 0, Palette.Length ).Select( i => Palette[( start + i ) % Palette.Length] )
			.FirstOrDefault( color => !used.Contains( color.ToArgb() & 0xFFFFFF ), Palette[start % Palette.Length] );
		var added = new CustomTileLayer();
		added.SetColor( color );
		RecordUndo();
		group.Layers.Add( added );
		RefreshList( groups.IndexOf( group ), group.Layers.Count - 1 );
		ShowPreview();
	}

	private void LayerRemoveButton_Click( object? sender, EventArgs e ) {
		if( Selected is not { Layers.Count: > 1 } group || SelectedLayer is not { } layer ) {
			return;
		}
		var index = group.Layers.IndexOf( layer );
		if( layer.Cells.Count > 0 && MessageBox.Show( this, $"「{group.Name}」のレイヤー {index + 1} を削除しますか? ({layer.Cells.Count} マス)", Text,
			MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) != DialogResult.OK ) {
			return;
		}
		RecordUndo();
		group.Layers.RemoveAt( index );
		RefreshList( groups.IndexOf( group ), Math.Min( index, group.Layers.Count - 1 ) );
		ShowPreview();
	}

	private void MoveLayer( int step ) {
		if( Selected is not { } group || SelectedLayer is not { } layer ) {
			return;
		}
		var index = group.Layers.IndexOf( layer );
		var target = index + step;
		if( target < 0 || target >= group.Layers.Count ) {
			return;
		}
		RecordUndo();
		( group.Layers[index], group.Layers[target] ) = ( group.Layers[target], group.Layers[index] );
		RefreshList( groups.IndexOf( group ), target );
		ShowPreview();
	}

	private void LayerUpButton_Click( object? sender, EventArgs e ) => MoveLayer( -1 );

	private void LayerDownButton_Click( object? sender, EventArgs e ) => MoveLayer( 1 );

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
		RefreshList( groups.IndexOf( group ), SelectedLayerIndex ); // 個別をやめたら全体の値を表示する
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
		RefreshLayerCounts();
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
