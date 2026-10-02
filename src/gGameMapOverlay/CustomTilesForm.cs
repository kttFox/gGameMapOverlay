using gGameMapOverlay.Overlay;

namespace gGameMapOverlay;

/// <summary>
/// 自分で描くマス (カスタム) の編集画面。グループごとに、マス・色・不透明度・表示するかを決める。
/// マスはキャラクターのいるマスからの相対位置で、オーバーレイではプレイヤーの枠と同じく画面に固定して描く。
/// 変更はその場で反映・保存する。開いたままゲームを操作できるよう、モードレスで表示する。
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
	// config を書き換えた後に呼ぶ (保存してオーバーレイを描き直す)。
	private readonly Action changed;
	// 背景に映すゲーム画面を撮る (画像と、その画面でのマスの幅)。ゲームが見つからなければ null。
	private readonly Func<Task<(Bitmap Image, double TileWidth)?>> captureGame;
	private bool refreshing;
	// 背景に映すゲーム画面のメッセージ (撮れなかった理由など)。
	private string gameStatus = "";

	public CustomTilesForm( AppConfig config, Action changed, Func<Task<(Bitmap Image, double TileWidth)?>> captureGame ) {
		this.config = config;
		this.changed = changed;
		this.captureGame = captureGame;
		InitializeComponent();
		opacityBox.Minimum = AppConfig.MinOverlayOpacity;
		opacityBox.Maximum = AppConfig.MaxOverlayOpacity;
		canvas.Groups = config.CustomGroups;
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
		RefreshList( config.CustomGroups.Count > 0 ? 0 : -1 );
	}

	/// <summary>ほかの画面 (メイン画面のチェック) でグループを変えた後に呼ぶ。選んでいるグループはそのまま。</summary>
	public void RefreshGroups() =>
		RefreshList( Selected is { } group ? config.CustomGroups.IndexOf( group ) : -1 );

	private CustomTileGroup? Selected =>
		groupList.SelectedIndices.Count > 0 && groupList.SelectedIndices[0] < config.CustomGroups.Count ? config.CustomGroups[groupList.SelectedIndices[0]] : null;

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
			foreach( var group in config.CustomGroups ) {
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
			foreach( var control in new Control[] { nameBox, colorButton, opacityBox, removeButton, clearButton } ) {
				control.Enabled = group is not null;
			}
			upButton.Enabled = group is not null && config.CustomGroups.IndexOf( group ) > 0;
			downButton.Enabled = group is not null && config.CustomGroups.IndexOf( group ) < config.CustomGroups.Count - 1;
			nameBox.Text = group?.Name ?? "";
			colorButton.BackColor = group is null ? SystemColors.Control : Color.FromArgb( 255, group.GetColor() );
			opacityBox.Value = Math.Clamp( group?.Opacity ?? AppConfig.DefaultOverlayOpacity, opacityBox.Minimum, opacityBox.Maximum );
		} finally {
			refreshing = false;
		}
		canvas.Selected = group;
		canvas.Invalidate();
		ShowStatus();
	}

	private void ShowStatus() {
		var hover = canvas.HoverCell is { } cell ? $"マウスの位置: キャラクターから X {cell.X:+0;-0;0}, Y {cell.Y:+0;-0;0}　" : "";
		var count = Selected is { } group ? $"「{group.Name}」 {group.Cells.Count} マス" : "グループなし (描くと作ります)";
		statusLabel.Text = hover + count + gameStatus;
	}

	protected override void OnShown( EventArgs e ) {
		base.OnShown( e );
		if( gameCheck.Checked ) {
			_ = CaptureGameAsync();
		}
	}

	protected override void OnFormClosed( FormClosedEventArgs e ) {
		base.OnFormClosed( e );
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
		while( config.CustomGroups.Any( group => group.Name == $"グループ {number}" ) ) {
			number++;
		}
		var added = new CustomTileGroup { Name = $"グループ {number}" };
		added.SetColor( Palette[config.CustomGroups.Count % Palette.Length] );
		config.CustomGroups.Add( added );
		RefreshList( config.CustomGroups.Count - 1 );
		changed();
		return added;
	}

	private void MoveGroup( int step ) {
		if( Selected is not { } group ) {
			return;
		}
		var index = config.CustomGroups.IndexOf( group );
		var target = index + step;
		if( target < 0 || target >= config.CustomGroups.Count ) {
			return;
		}
		( config.CustomGroups[index], config.CustomGroups[target] ) = ( config.CustomGroups[target], config.CustomGroups[index] );
		RefreshList( target );
		changed();
	}

	// ---- イベントハンドラ (デザイナーから接続) ------------------------------------------

	private void GroupList_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( !refreshing ) {
			ShowSelected();
		}
	}

	private void GroupList_ItemChecked( object? sender, ItemCheckedEventArgs e ) {
		if( refreshing || e.Item.Index >= config.CustomGroups.Count ) {
			return;
		}
		config.CustomGroups[e.Item.Index].Shown = e.Item.Checked;
		canvas.Invalidate();
		changed();
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
		var index = config.CustomGroups.IndexOf( group );
		config.CustomGroups.RemoveAt( index );
		RefreshList( Math.Min( index, config.CustomGroups.Count - 1 ) );
		changed();
	}

	private void UpButton_Click( object? sender, EventArgs e ) => MoveGroup( -1 );

	private void DownButton_Click( object? sender, EventArgs e ) => MoveGroup( 1 );

	private void NameBox_TextChanged( object? sender, EventArgs e ) {
		if( refreshing || Selected is not { } group ) {
			return;
		}
		group.Name = nameBox.Text;
		groupList.Items[config.CustomGroups.IndexOf( group )].Text = group.Name;
		ShowStatus();
		changed();
	}

	private void ColorButton_Click( object? sender, EventArgs e ) {
		if( Selected is not { } group ) {
			return;
		}
		using var dialog = new ColorDialog { Color = Color.FromArgb( 255, group.GetColor() ), FullOpen = true };
		if( dialog.ShowDialog( this ) != DialogResult.OK ) {
			return;
		}
		group.SetColor( dialog.Color );
		RefreshList( config.CustomGroups.IndexOf( group ) );
		changed();
	}

	private void OpacityBox_ValueChanged( object? sender, EventArgs e ) {
		if( refreshing || Selected is not { } group ) {
			return;
		}
		group.Opacity = (int)opacityBox.Value;
		canvas.Invalidate();
		changed();
	}

	private void ThicknessBox_SelectedIndexChanged( object? sender, EventArgs e ) {
		if( thicknessBox.SelectedIndex < 0 ) {
			return;
		}
		canvas.Thickness = CustomTileShape.MinThickness + thicknessBox.SelectedIndex;
		RefreshShapeImages();
	}

	private void ClearButton_Click( object? sender, EventArgs e ) {
		if( Selected is not { } group || group.Cells.Count == 0 ) {
			return;
		}
		if( MessageBox.Show( this, $"「{group.Name}」のマスを全部消しますか? ({group.Cells.Count} マス)", Text,
			MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) != DialogResult.OK ) {
			return;
		}
		group.Cells.Clear();
		canvas.Invalidate();
		ShowStatus();
		changed();
	}

	private void Canvas_CellsChanged( object? sender, EventArgs e ) {
		ShowStatus();
		changed();
	}

	private void Canvas_HoverChanged( object? sender, EventArgs e ) => ShowStatus();

	private void CloseButton_Click( object? sender, EventArgs e ) => Close();

	private void GameCheck_CheckedChanged( object? sender, EventArgs e ) {
		if( refreshing ) {
			return;
		}
		config.CustomEditorShowGame = gameCheck.Checked;
		changed();
		recaptureButton.Enabled = gameCheck.Checked;
		if( gameCheck.Checked ) {
			_ = CaptureGameAsync();
		} else {
			gameStatus = "";
			canvas.SetGameImage( null, 0 );
			ShowStatus();
		}
	}

	private void RecaptureButton_Click( object? sender, EventArgs e ) => _ = CaptureGameAsync();
}
