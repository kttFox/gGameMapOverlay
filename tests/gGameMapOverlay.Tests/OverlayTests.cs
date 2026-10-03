using System.Drawing;
using gGameMapOverlay.Overlay;
using gGameMapOverlay.Parsing;

namespace gGameMapOverlay.Tests;

public class IsoGridTests
{
    private static readonly IsoGrid Grid = new(new PointF(400, 300), new GameCoordinate(40, 40));

    [Fact]
    public void Tile_OfPlayerIsCenteredOnPlayerCenter()
    {
        // 上・右・下・左の頂点 (幅 64 × 高さ 32 の菱形)
        Assert.Equal([new PointF(400, 284), new PointF(432, 300), new PointF(400, 316), new PointF(368, 300)], Grid.Tile(40, 40));
    }

    [Theory]
    [InlineData(41, 40, 432, 316)] // x が 1 増えると右下
    [InlineData(40, 41, 368, 316)] // y が 1 増えると左下
    [InlineData(41, 41, 400, 332)] // 両方増えると真下
    [InlineData(39, 41, 336, 300)] // 真左
    public void Tile_NeighborsFollowIsometricLayout(int x, int y, float centerX, float centerY)
    {
        var tile = Grid.Tile(x, y);
        Assert.Equal(new PointF(centerX, centerY), new PointF((tile[1].X + tile[3].X) / 2, (tile[0].Y + tile[2].Y) / 2));
    }

    [Fact]
    public void TileSize_ScalesAroundPlayerCenter()
    {
        // マスを 1.25 倍 (80 × 40) にすると、キャラクターのマスは中央のまま、離れたマスほど外へ動く
        var grid = Grid with { TileWidth = 80, TileHeight = 40 };
        Assert.Equal(new PointF(400, 280), grid.Tile(40, 40)[0]);
        var far = grid.Tile(50, 40); // 右下へ 10 マス: 原寸なら中心 (720, 460)
        Assert.Equal(new PointF(800, 500), new PointF((far[1].X + far[3].X) / 2, (far[0].Y + far[2].Y) / 2));
    }

    [Fact]
    public void Polygon_CoversCornerTilesOfRect()
    {
        // [x, y, 幅, 高さ] = [40, 40, 3, 2] は、(40,40) の上の頂点・(42,40) の右の頂点・(42,41) の下の頂点・(40,41) の左の頂点を結ぶ
        var polygon = Grid.Polygon(40, 40, 3, 2);
        Assert.Equal(Grid.Tile(40, 40)[0], polygon[0]);
        Assert.Equal(Grid.Tile(42, 40)[1], polygon[1]);
        Assert.Equal(Grid.Tile(42, 41)[2], polygon[2]);
        Assert.Equal(Grid.Tile(40, 41)[3], polygon[3]);
    }

    [Theory]
    [InlineData(400, 300, 40, 40)] // キャラクターのマスの中心
    [InlineData(431, 300, 40, 40)] // 右の頂点の少し内側
    [InlineData(433, 300, 41, 39)] // 右の頂点の少し外側は、右 (x+1, y-1) のマス
    [InlineData(432, 316, 41, 40)] // 右下 (x+1) のマスの中心
    [InlineData(368, 316, 40, 41)] // 左下 (y+1) のマスの中心
    [InlineData(400, 236, 38, 38)] // 真上に 2 マス
    public void TileAt_FindsTileUnderPoint(float x, float y, int tileX, int tileY)
    {
        Assert.Equal(new GameCoordinate(tileX, tileY), Grid.TileAt(new PointF(x, y)));
    }
}

public class GameUiTests
{
    [Theory]
    [InlineData(1056, 792, 200, 12, true)]   // マップ名欄
    [InlineData(1056, 792, 40, 200, true)]   // SKILL 欄
    [InlineData(1056, 792, 950, 40, false)]  // 右上の HP/MP は動かせて、上を右クリックしても歩くので除かない
    [InlineData(1056, 792, 528, 720, true)]  // チャット欄
    [InlineData(1056, 792, 1000, 770, true)] // 右下
    [InlineData(1056, 792, 528, 396, false)] // キャラクター
    [InlineData(1056, 792, 40, 700, false)]  // 左下の UI のない所
    [InlineData(1500, 792, 750, 720, true)]  // 横に広げても、チャット欄は中央
    [InlineData(1500, 792, 100, 760, false)] // 下の左の欄は中央に付いてくるので、左端は空く
    [InlineData(1056, 700, 528, 640, true)]  // 縦を縮めても、チャット欄は下端
    public void Contains_FixedUi(int width, int height, int x, int y, bool expected)
    {
        Assert.Equal(expected, gGameMapOverlay.Imaging.GameUi.Contains(new Size(width, height), gGameMapOverlay.Imaging.UiTransform.Identity, new Point(x, y)));
    }

    [Fact]
    public void Contains_ScalesWithUi()
    {
        // UI が 1.5 倍なら、マップ名欄も 1.5 倍の幅まで
        var scaled = new gGameMapOverlay.Imaging.UiTransform(1.5);
        Assert.True(gGameMapOverlay.Imaging.GameUi.Contains(new Size(1920, 1620), scaled, new Point(500, 30)));
        Assert.False(gGameMapOverlay.Imaging.GameUi.Contains(new Size(1920, 1620), gGameMapOverlay.Imaging.UiTransform.Identity, new Point(500, 30)));
    }
}

public class ClickDirectionTests
{
    [Theory]
    [InlineData(1, 0, 1, 0)]   // 隣のマスはその方向
    [InlineData(0, -3, 0, -1)] // 遠くても軸の上ならその方向
    [InlineData(3, 2, 1, 0)]   // 45° の線より内側は、ずれの大きい方の軸
    [InlineData(-2, -3, 0, -1)]
    public void Straight_MovesAlongLargerAxis(int tileX, int tileY, int x, int y)
    {
        Assert.Equal((x, y), MainForm.ClickDirection(tileX, tileY, null));
    }

    [Theory]
    [InlineData(1, 1, 0, -1, 0, 1)]    // 上 (y-1) を向いていて右下 (x+1,y+1) の斜め: 後ろ (y+1)
    [InlineData(1, -1, 0, -1, 0, -1)]  // 上を向いていて右上 (x+1,y-1) の斜め: 前 (y-1)
    [InlineData(-2, 2, 1, 0, -1, 0)]   // x+1 を向いていて (-2,2) の斜め: 後ろ (x-1)
    [InlineData(2, 2, -1, 0, 1, 0)]    // x-1 を向いていて (2,2) の斜め: 後ろ (x+1)
    public void Diagonal_MovesForwardOrBackAlongFacing(int tileX, int tileY, int facingX, int facingY, int x, int y)
    {
        Assert.Equal((x, y), MainForm.ClickDirection(tileX, tileY, (facingX, facingY)));
    }

    [Fact]
    public void NoDirection_OnPlayerTileOrDiagonalWithoutFacing()
    {
        Assert.Null(MainForm.ClickDirection(0, 0, (1, 0)));
        Assert.Null(MainForm.ClickDirection(1, 1, null));
    }
}

public class OverlayDataTests
{
    private static readonly OverlayData Data = OverlayData.Load(OverlayData.DefaultDirectory);

    [Theory]
    [InlineData("ディーバル", "10_deeval")]
    [InlineData("ディバル", "10_deeval")]
    [InlineData("レゲ地下洞窟 ９Ｆ", "rege9f")] // OCR の空白・全角の揺れ
    [InlineData("레게지하동굴9층", "rege9f")]
    [InlineData("Rege B Cave 9F", "rege9f")]
    public void FindMapId_MatchesAnyLanguageAlias(string name, string id)
    {
        Assert.Equal(id, Data.FindMapId(name));
    }

    [Fact]
    public void FindMapId_ReturnsNullForUnknownName()
    {
        Assert.Null(Data.FindMapId("インベントリ"));
        Assert.Null(Data.FindMapId(null));
    }

    [Fact]
    public void MonsterBlock_ReadsRects()
    {
        var map = Data.Find(OverlayLayer.MonsterBlock, "calbiron");
        Assert.NotNull(map);
        Assert.Equal("カルビロンの森", map.Name);
        Assert.Equal([62, 0, 1, 10], map.Rects[0]);
        Assert.True(map.Contains(62, 9));
        Assert.True(map.Contains(66, 10)); // [62, 10, 5, 1]
        Assert.False(map.Contains(63, 5));
    }

    [Fact]
    public void Towns_HaveEdgesButNoMonsterBlock()
    {
        // 街にはモンスター境界が無い (monster-block.md)。壁・出入口・特殊マスはある。
        Assert.Null(Data.Find(OverlayLayer.MonsterBlock, "10_deeval"));
        Assert.NotNull(Data.Find(OverlayLayer.ImpassableEdge, "10_deeval"));
        Assert.NotNull(Data.Find(OverlayLayer.MapMove, "10_deeval"));
        Assert.NotNull(Data.Find(OverlayLayer.Special, "10_deeval"));
    }

    [Fact]
    public void Load_ReadsEveryLayerFile()
    {
        // 件数はデータを作り直すと変わるので、どの種類も読めていることだけ確かめる
        Assert.Empty(Data.MissingLayers);
        Assert.All(OverlayLayer.All, layer => Assert.True(Data.MapCountOf(layer) > 0));
        Assert.Null(Data.Find(OverlayLayer.ImpassableEdge, "maze1")); // データが見つかっていないマップ (data/*.md)
    }

    [Fact]
    public void Load_SkipsMissingLayerFiles()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            File.Copy(Path.Combine(OverlayData.DefaultDirectory, OverlayData.MapsFileName), Path.Combine(directory.FullName, OverlayData.MapsFileName));
            File.WriteAllText(Path.Combine(directory.FullName, OverlayLayer.Special.FileName), """{"hell1f": {"name": "ヘル1F", "rects": [[1, 2, 3, 1]]}}""");
            var data = OverlayData.Load(directory.FullName);
            Assert.Equal(OverlayLayer.All.Where(layer => layer != OverlayLayer.Special), data.MissingLayers);
            Assert.Null(data.Find(OverlayLayer.MonsterBlock, "hell1f"));
            Assert.True(data.Find(OverlayLayer.Special, "hell1f")?.Contains(3, 2));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}

public class DataCipherTests
{
    [Fact]
    public void Decrypt_RoundTrips()
    {
        const string text = """{"hell1f": {"name": "ヘル1F"}}""";
        Assert.Equal(text, DataCipher.Decrypt(DataCipher.Encrypt(text)));
    }

    [Fact]
    public void Decrypt_RejectsTamperedData()
    {
        var data = DataCipher.Encrypt("{}");
        data[^1] ^= 1;
        Assert.ThrowsAny<System.Security.Cryptography.CryptographicException>(() => DataCipher.Decrypt(data));
    }
}

public class OverlayConfigTests
{
    // 色の RGB だけ (アルファは透明度から決まる)。
    private static int Rgb(Color color) => color.ToArgb() & 0xFFFFFF;

    [Fact]
    public void Colors_DefaultToLayerAndCanBeChanged()
    {
        var config = new AppConfig();
        Assert.Equal(Rgb(Color.FromArgb(40, 120, 255)), Rgb(config.GetOverlayColor(OverlayLayer.MonsterBlock))); // 青
        Assert.Equal(Rgb(Color.FromArgb(230, 40, 40)), Rgb(config.GetOverlayColor(OverlayLayer.ImpassableEdge))); // 赤
        config.SetOverlayColor(OverlayLayer.MapMove, Color.Orange);
        Assert.Equal("#FFA500", config.OverlayColors["map_move"]);
        Assert.Equal(Rgb(Color.Orange), Rgb(config.Clone().GetOverlayColor(OverlayLayer.MapMove))); // 保存・読み込みで残る
        config.SetOverlayColor(OverlayLayer.MapMove, OverlayLayer.MapMove.DefaultColor);
        Assert.Empty(config.OverlayColors); // 初期値に戻したら設定から消える
    }

    [Fact]
    public void Colors_IgnoreBadValues()
    {
        var config = new AppConfig();
        config.OverlayColors["special"] = "not a color";
        Assert.Equal(Rgb(OverlayLayer.Special.DefaultColor), Rgb(config.GetOverlayColor(OverlayLayer.Special)));
    }

    [Fact]
    public void Opacity_OwnOrOverall()
    {
        var config = new AppConfig { OverlayOpacity = 60 };
        Assert.Equal(60, config.GetOverlayOpacity(OverlayLayer.Special)); // 個別がなければ全体
        Assert.Equal(153, config.GetOverlayColor(OverlayLayer.Special).A); // 255 × 60%
        config.SetOwnOpacity("special", 100);
        config.SetOwnOpacity(AppConfig.GridColorKey, 60); // 全体と同じ値でも個別のまま
        Assert.Equal(255, config.Clone().GetOverlayColor(OverlayLayer.Special).A); // 不透明。保存・読み込みで残る
        Assert.True(config.Clone().HasOwnOpacity(AppConfig.GridColorKey));
        config.OverlayOpacity = 30;
        Assert.Equal(30, config.GetOverlayOpacity(OverlayLayer.MonsterBlock)); // 全体を変えると個別でない色は変わる
        Assert.Equal(60, config.GetGridOpacity()); // 個別の色は変わらない
        config.ClearOwnOpacity("special");
        Assert.Equal(30, config.GetOverlayOpacity(OverlayLayer.Special)); // 個別をやめたら全体
        config.SetOwnOpacity("special", 0);
        Assert.Equal(AppConfig.MinOverlayOpacity, config.GetOverlayOpacity(OverlayLayer.Special)); // 範囲に丸める (見えなくならない)
    }

    [Theory]
    [InlineData(660, 64.0, 32.0)]
    [InlineData(768, 64.0, 32.0)]    // 768 までは原寸
    [InlineData(960, 80.0, 40.0)]    // 768 を超えたら 高さ ÷ 768 倍
    [InlineData(1080, 90.0, 45.0)]   // 実機で合った大きさ (フルスクリーン 1920 × 1080 で 90 × 45)
    public void ForClient_GrowsTilesAboveClientHeight768(int clientHeight, double width, double height)
    {
        var grid = IsoGrid.ForClient(new Size(1920, clientHeight), new GameCoordinate(40, 40));
        Assert.Equal((width, height), (grid.TileWidth, grid.TileHeight));
        Assert.Equal(new PointF(960, clientHeight / 2f), grid.PlayerCenter); // キャラクターは画面の中央
    }

    [Fact]
    public void ForClient_MatchesWindowedMeasurement()
    {
        // 実機のウィンドウ表示 (クライアント 1444 × 861) では、0.5 刻みの目視で 71.5〜72 × 36 が合った
        var grid = IsoGrid.ForClient(new Size(1444, 861), new GameCoordinate(40, 40));
        Assert.InRange(grid.TileWidth, 71.5, 72.0);
        Assert.InRange(grid.TileHeight, 35.75, 36.25);
    }

    [Fact]
    public void Load_IgnoresRemovedPositionSettings()
    {
        // 以前の版で保存された位置の設定 (マスの大きさ・ずらす量など) が残っていても読み込める
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var path = Path.Combine(directory.FullName, "config.json");
            File.WriteAllText(path, """{"overlay_tile_width": 70, "overlay_offset_x": 5, "overlay_swap_xy": true, "overlay_tile_scaling": "none", "overlay_tile_base_height": 600, "ocr_language": "ko"}""");
            Assert.Equal("ko", AppConfig.Load(path).OcrLanguage);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Layers_AreShownUnlessTurnedOff()
    {
        var config = new AppConfig();
        Assert.All(OverlayLayer.All, layer => Assert.True(config.IsOverlayLayerShown(layer)));
        config.SetOverlayLayerShown(OverlayLayer.Special, false);
        Assert.False(config.Clone().IsOverlayLayerShown(OverlayLayer.Special)); // 保存・読み込みで残る
        Assert.True(config.OverlayEnabled);
        foreach (var layer in OverlayLayer.All)
        {
            config.SetOverlayLayerShown(layer, false);
        }
        Assert.True(config.OverlayEnabled); // プレイヤーの枠は出す
        config.ShowPlayer = false;
        Assert.False(config.OverlayEnabled);
    }

    [Fact]
    public void CustomGroups_ShownOnlyWhenCheckedAndNotEmpty()
    {
        var config = new AppConfig { ShowPlayer = false };
        foreach (var layer in OverlayLayer.All)
        {
            config.SetOverlayLayerShown(layer, false);
        }
        var empty = new CustomTileGroup { Name = "空", Layers = [new()] };
        var drawn = new CustomTileGroup { Name = "範囲", Layers = [new(), new() { Cells = [[1, 0], [0, -1]] }] };
        config.CustomGroups.AddRange([empty, drawn]);
        Assert.Equal([drawn], config.ShownCustomGroups);
        Assert.True(config.OverlayEnabled); // カスタムだけでも表示する
        drawn.Shown = false;
        Assert.Empty(config.ShownCustomGroups);
        Assert.False(config.OverlayEnabled);
    }

    [Fact]
    public void CustomGroups_RoundTripAndNormalize()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var path = Path.Combine(directory.FullName, "config.json");
            File.WriteAllText(path, """{"custom_groups": [{"name": "A", "color": "#FF8000", "opacity": 500, "shown": false, "cells": [[1, 2], [1, 2], [3], [-4, 5], [0, 0, 0], [7, 7, 511], [8, 8, 16]]}, null]}""");
            var group = Assert.Single(AppConfig.Load(path).CustomGroups);
            Assert.Equal("A", group.Name);
            Assert.False(group.Shown);
            Assert.Equal(AppConfig.MaxOverlayOpacity, group.Opacity); // 範囲に収める
            var layer = Assert.Single(group.Layers); // レイヤーがなかったころの色・マスは 1 つ目のレイヤーにする
            Assert.Equal([[1, 2], [-4, 5], [7, 7], [8, 8, 16]], layer.Cells); // 重複・壊れたマス・形が空のマスを除き、1 マス全部は形を書かない
            Assert.Equal(Color.FromArgb(255, 255, 128, 0), layer.GetColor(100));
            Assert.Null(group.LegacyCells);
            Assert.Null(group.LegacyColor);
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void CustomGroups_LayersRoundTrip()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var path = Path.Combine(directory.FullName, "config.json");
            var config = new AppConfig();
            config.CustomGroups.Add(new CustomTileGroup { Name = "A", Layers = [new() { Color = "#FF0000", Cells = [[0, 1]] }, new() { Color = "#0000FF", Cells = [[2, 3, CustomTileGroup.FrameFlag]] }] });
            config.Save(path);
            Assert.DoesNotContain("\"color\": \"#00BCD4\"", File.ReadAllText(path)); // グループには色を書かない
            var group = Assert.Single(AppConfig.Load(path).CustomGroups);
            Assert.Equal(["#FF0000", "#0000FF"], group.Layers.Select(layer => layer.Color));
            Assert.Equal([[2, 3, CustomTileGroup.FrameFlag]], group.Layers[1].Cells);
            Assert.Equal(2, group.CellCount);
            Assert.Equal(2, group.ToCustomTiles().Count()); // レイヤーごとに描く
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void CustomGroup_ColorUsesOwnOpacity()
    {
        var layer = new CustomTileLayer { Color = "#102030", Cells = [[0, 0]] };
        var group = new CustomTileGroup { Opacity = 40, Layers = [layer] };
        Assert.Equal(Color.FromArgb(102, 0x10, 0x20, 0x30), Assert.Single(group.ToCustomTiles()).Color);
        group.OwnOpacity = false;
        Assert.Equal(204, Assert.Single(group.ToCustomTiles(80)).Color.A); // 個別でなければ全体の不透明度
        layer.Color = "bad";
        Assert.Equal(CustomTileGroup.DefaultColor.ToArgb() & 0xFFFFFF, layer.GetColor().ToArgb() & 0xFFFFFF);
    }

    [Theory]
    // 左上なら ■■■ / ■□□ / ■□□ (上の列と左の列)
    [InlineData("左上", "111/100/100")]
    [InlineData("右上", "111/001/001")]
    [InlineData("右下", "001/001/111")]
    [InlineData("左下", "100/100/111")]
    // 丁字 (上) なら ■■■ / □■□ / □■□ (上の列と真ん中の縦の列)
    [InlineData("丁字 (上)", "111/010/010")]
    [InlineData("丁字 (右)", "001/111/001")]
    [InlineData("丁字 (下)", "010/010/111")]
    [InlineData("丁字 (左)", "100/111/100")]
    public void CustomShape_MatchesPattern(string name, string rows)
    {
        var expected = rows.Split('/').SelectMany((row, sy) => row.Select((c, sx) => c == '1' ? CustomTileShape.PartMask(sx, sy) : 0)).Sum();
        Assert.Equal(expected, CustomTileShape.All.Single(shape => shape.Name == name).Mask);
    }

    [Fact]
    public void CustomShape_HasNoEndCapsOrCross() => Assert.DoesNotContain(CustomTileShape.All, shape => shape.Name.StartsWith("端") || shape.Name == "十字");

    [Fact]
    public void CustomGroup_SetCellAddsAndRemoves()
    {
        var group = new CustomTileLayer();
        Assert.True(group.SetCell(2, -1, CustomTileGroup.FullMask));
        Assert.False(group.SetCell(2, -1, CustomTileGroup.FullMask)); // 2 回描いても 1 つ
        Assert.Equal(CustomTileGroup.FullMask, group.MaskAt(2, -1));
        Assert.Equal([[6, -3, 3, 3]], group.ToTileRects().Rects); // 1/3 マス単位
        Assert.Equal([[2, -1]], group.Cells); // 1 マス全部は形を書かない
        var corner = CustomTileShape.PartMask(0, 0);
        Assert.True(group.SetCell(2, -1, corner)); // 形は置き換える
        Assert.Equal([[6, -3, 1, 1]], group.ToTileRects().Rects);
        Assert.True(group.SetCell(2, -1, 0));
        Assert.Empty(group.Cells);
    }

    [Theory]
    // 縦: 真ん中の 1/thickness の幅 (偶数なら 1/(2 × thickness) 単位で中央に置く)
    [InlineData("縦", 2, 4, "1,0,2,4")]
    [InlineData("縦", 3, 3, "1,0,1,3")]
    [InlineData("縦", 4, 8, "3,0,2,8")]
    [InlineData("縦", 5, 5, "2,0,1,5")]
    [InlineData("縦", 16, 32, "15,0,2,32")]
    // 左上: 上の列と左の列の L 字
    [InlineData("左上", 5, 5, "0,0,5,1/0,1,1,4")]
    // 丁字 (上): 細いと上の列と真ん中の列の間に隙間ができるが、両方塗るのでつなげる
    [InlineData("丁字 (上)", 5, 5, "0,0,5,1/2,1,1,4")]
    // 太枠: 縁から 1/thickness の輪
    [InlineData("太枠", 4, 4, "0,0,4,1/0,1,1,2/3,1,1,2/0,3,4,1")]
    // 1 マス全部は太さで変わらない
    [InlineData("1 マス", 5, 1, "0,0,1,1")]
    public void CustomShape_FillsWithThickness(string name, int thickness, int division, string rects)
    {
        var fill = CustomTileShape.Fill(CustomTileShape.All.Single(shape => shape.Name == name).Mask, thickness);
        Assert.Equal(division, fill.Division);
        Assert.Equal(rects.Split('/').Select(rect => rect.Split(',').Select(int.Parse).ToArray()), fill.Rects);
    }

    [Fact]
    public void CustomGroup_KeepsThicknessPerCell()
    {
        var vertical = CustomTileShape.All.Single(shape => shape.Name == "縦").Mask;
        var group = new CustomTileLayer();
        Assert.True(group.SetCell(0, 0, vertical));
        Assert.Equal([[0, 0, vertical]], group.Cells); // 太さ 1/3 は書かない
        Assert.True(group.SetCell(0, 0, vertical, 5)); // 太さだけ変えても描き直す
        Assert.False(group.SetCell(0, 0, vertical, 5));
        Assert.Equal(5, group.ThicknessAt(0, 0));
        Assert.True(group.SetCell(1, 0, CustomTileGroup.FullMask, 5));
        Assert.Equal([1, 0], group.Cells[1]); // 1 マス全部は太さを書かない
        // 1/3 と 1/5 が両方割り切れる 1/15 マス単位で描く
        Assert.Equal(15, group.FillDivision());
        Assert.Equal([[6, 0, 3, 15], [15, 0, 15, 15]], group.ToTileRects().Rects);
        Assert.Equal(15, group.ToCustomTiles().Division);
    }

    [Fact]
    public void CustomGroup_NormalizeKeepsThickness()
    {
        var group = new CustomTileLayer { Cells = [[0, 0, 0b010_010_010, 5], [1, 0, 0b010_010_010, 3], [2, 0, CustomTileGroup.FullMask, 4], [3, 0, 0b010_010_010, 99]] };
        group.Normalize();
        Assert.Equal([[0, 0, 0b010_010_010, 5], [1, 0, 0b010_010_010], [2, 0], [3, 0, 0b010_010_010, CustomTileShape.MaxThickness]], group.Cells);
    }
}

public class OverlayDrawingTests
{
    private static readonly Color Background = Color.FromArgb(255, 0, 255);
    private static readonly Size ClientSize = new(800, 600);

    private static Bitmap Render(OverlayScene scene)
    {
        var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Background);
        OverlayForm.Draw(graphics, scene, ClientSize);
        return bitmap;
    }

    private static (OverlayLayer, TileRects, Color) Entry(OverlayLayer layer, TileRects tiles) => (layer, tiles, layer.DefaultColor);

    private static OverlayScene Scene(TileRects? monsterBlock, TileRects? edge = null, Rectangle? excluded = null)
    {
        var layers = new List<(OverlayLayer, TileRects, Color)>();
        if (edge is not null)
        {
            layers.Add(Entry(OverlayLayer.ImpassableEdge, edge));
        }
        if (monsterBlock is not null)
        {
            layers.Add(Entry(OverlayLayer.MonsterBlock, monsterBlock));
        }
        return new(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), new IsoGrid(new PointF(400, 300), new GameCoordinate(62, 5)),
            layers, excluded, null, PlayerColor: AppConfig.DefaultPlayerColor);
    }

    private static readonly TileRects Blocks = new() { Rects = [[62, 0, 1, 10]] }; // (62, 0) 〜 (62, 9) の縦一列
    private static readonly TileRects Edges = new() { Rects = [[64, 5, 1, 1]] };

    [Fact]
    public void Draw_FillsBlockedTilesOnly()
    {
        using var bitmap = Render(Scene(Blocks, Edges));
        Assert.Equal(OverlayLayer.MonsterBlock.DefaultColor.ToArgb(), bitmap.GetPixel(400, 300).ToArgb()); // 足元 (62, 5) はモンスター境界
        Assert.Equal(OverlayLayer.MonsterBlock.DefaultColor.ToArgb(), bitmap.GetPixel(400 + 32, 300 - 16).ToArgb()); // (62, 4) は右上
        Assert.Equal(Background.ToArgb(), bitmap.GetPixel(400 + 32, 300 + 16).ToArgb()); // (63, 5) は通常
        Assert.Equal(OverlayLayer.ImpassableEdge.DefaultColor.ToArgb(), bitmap.GetPixel(400 + 64, 300 + 32).ToArgb()); // (64, 5) は壁
    }

    [Fact]
    public void Draw_LaterLayersOnTop()
    {
        // 同じマスに重なったら、後ろの種類 (OverlayLayer.All の後ろ) の色になる
        var tile = new TileRects { Rects = [[62, 5, 1, 1]] };
        var scene = Scene(null) with { Layers = [Entry(OverlayLayer.ImpassableEdge, tile), Entry(OverlayLayer.MapMove, tile)] };
        using var bitmap = Render(scene);
        Assert.Equal(OverlayLayer.MapMove.DefaultColor.ToArgb(), bitmap.GetPixel(400, 300).ToArgb());
    }

    [Fact]
    public void Draw_LayerOrderIsMonsterBlockSpecialEdgeMapMove()
    {
        Assert.Equal([OverlayLayer.MonsterBlock, OverlayLayer.Special, OverlayLayer.ImpassableEdge, OverlayLayer.MapMove], OverlayLayer.All);
        // すべての種類が同じマスに重なったら、一番上の出入口の色になる
        var tile = new TileRects { Rects = [[62, 5, 1, 1]] };
        var scene = Scene(null) with { Layers = OverlayLayer.All.Select(layer => Entry(layer, tile)).ToList() };
        using var bitmap = Render(scene);
        Assert.Equal(OverlayLayer.MapMove.DefaultColor.ToArgb(), bitmap.GetPixel(400, 300).ToArgb());
    }

    [Fact]
    public void Draw_ExitHidesWallOnSameTile()
    {
        // 壁と出入口が重なったマスは出入口の色だけにする (半透明でも色が混ざらない)
        var wall = new TileRects { Rects = [[61, 5, 3, 1]] }; // (61, 5) 〜 (63, 5)
        var exit = new TileRects { Rects = [[62, 5, 1, 1]] };
        var half = (OverlayLayer layer) => Color.FromArgb(128, layer.DefaultColor);
        var scene = Scene(null) with { Layers = [(OverlayLayer.ImpassableEdge, wall, half(OverlayLayer.ImpassableEdge)), (OverlayLayer.MapMove, exit, half(OverlayLayer.MapMove))] };
        using var bitmap = Render(scene);
        using var exitOnly = Render(Scene(null) with { Layers = [(OverlayLayer.MapMove, exit, half(OverlayLayer.MapMove))] });
        Assert.Equal(exitOnly.GetPixel(400, 300).ToArgb(), bitmap.GetPixel(400, 300).ToArgb()); // 重なったマスは出入口だけ
        Assert.NotEqual(Background.ToArgb(), bitmap.GetPixel(400 - 32, 300 - 16).ToArgb()); // (61, 5) の壁は描く
        // 出入口を表示していなければ壁を描く
        using var wallOnly = Render(Scene(null) with { Layers = [(OverlayLayer.ImpassableEdge, wall, half(OverlayLayer.ImpassableEdge))] });
        Assert.NotEqual(Background.ToArgb(), wallOnly.GetPixel(400, 300).ToArgb());
    }

    [Fact]
    public void Draw_OverlappingTilesDoNotMixColors()
    {
        // 半透明でも、重なったマスは後から描いた色だけになる (縮めた特殊マスも、カスタムのマスも)
        var half = (OverlayLayer layer) => Color.FromArgb(128, layer.DefaultColor);
        var tile = new TileRects { Rects = [[62, 5, 1, 1]] };
        using var both = Render(Scene(null) with { PlayerColor = null, Layers = [(OverlayLayer.MonsterBlock, tile, half(OverlayLayer.MonsterBlock)), (OverlayLayer.Special, tile, half(OverlayLayer.Special))] });
        using var special = Render(Scene(null) with { PlayerColor = null, Layers = [(OverlayLayer.Special, tile, half(OverlayLayer.Special))] });
        using var monster = Render(Scene(null) with { PlayerColor = null, Layers = [(OverlayLayer.MonsterBlock, tile, half(OverlayLayer.MonsterBlock))] });
        Assert.Equal(special.GetPixel(400, 300).ToArgb(), both.GetPixel(400, 300).ToArgb()); // 縮めた特殊マスの中
        Assert.Equal(monster.GetPixel(400, InsideFullTileOnly).ToArgb(), both.GetPixel(400, InsideFullTileOnly).ToArgb()); // 周りはモンスター境界

        var custom = new List<CustomTiles> { new CustomTileLayer { Cells = [[0, 0]] }.ToCustomTiles() };
        using var over = Render(Scene(null) with { PlayerColor = null, Layers = [(OverlayLayer.ImpassableEdge, tile, half(OverlayLayer.ImpassableEdge))], Custom = custom });
        using var customOnly = Render(Scene(null) with { PlayerColor = null, Custom = custom });
        Assert.Equal(customOnly.GetPixel(400, 300).ToArgb(), over.GetPixel(400, 300).ToArgb());
    }

    // 足元 (62, 5) の菱形は上の頂点が y = 284。0.6 倍に縮めると y = 290.4 なので、y = 287 は縮めたときだけ外になる。
    private const int InsideFullTileOnly = 287;

    [Fact]
    public void Draw_ShrinksSpecialTilesOverMonsterBlock()
    {
        var monster = new TileRects { Rects = [[62, 5, 1, 1]] };
        var special = new TileRects { Rects = [[61, 5, 2, 1]] }; // (61, 5) は重ならない・(62, 5) は重なる
        var scene = Scene(null) with { Layers = [Entry(OverlayLayer.MonsterBlock, monster), Entry(OverlayLayer.Special, special)] };
        using var bitmap = Render(scene);
        Assert.Equal(OverlayLayer.Special.DefaultColor.ToArgb(), bitmap.GetPixel(400, 300).ToArgb()); // 中心は特殊マス
        Assert.Equal(OverlayLayer.MonsterBlock.DefaultColor.ToArgb(), bitmap.GetPixel(400, InsideFullTileOnly).ToArgb()); // 周りにモンスター境界が見える
        // 重ならない (61, 5) は元の大きさ (中心 (368, 284))
        Assert.Equal(OverlayLayer.Special.DefaultColor.ToArgb(), bitmap.GetPixel(368, 284 - 300 + InsideFullTileOnly).ToArgb());
    }

    [Fact]
    public void Draw_KeepsSpecialTilesFullSizeWhenMonsterBlockHidden()
    {
        // モンスター境界を表示していなければ縮めない
        var special = new TileRects { Rects = [[62, 5, 1, 1]] };
        var scene = Scene(null) with { Layers = [Entry(OverlayLayer.Special, special)] };
        using var bitmap = Render(scene);
        Assert.Equal(OverlayLayer.Special.DefaultColor.ToArgb(), bitmap.GetPixel(400, InsideFullTileOnly).ToArgb());
    }

    [Fact]
    public void Scene_ComparesLayersByContent()
    {
        // 読み取りのたびに作り直しても、内容が同じなら描き直さない
        Assert.Equal(Scene(Blocks, Edges), Scene(Blocks, Edges));
        Assert.NotEqual(Scene(Blocks, Edges), Scene(Blocks));
    }

    [Fact]
    public void Draw_OutlinesPlayerTile()
    {
        using var bitmap = Render(Scene(null));
        Assert.Equal(AppConfig.DefaultPlayerColor.ToArgb(), bitmap.GetPixel(416, 292).ToArgb()); // 足元のマスの右上の辺
        Assert.Equal(Background.ToArgb(), bitmap.GetPixel(400, 300).ToArgb());
    }

    private static readonly Color CustomColor = Color.FromArgb(255, CustomTileGroup.DefaultColor);

    /// <summary>不透明の 1 グループ。cells は [dx, dy] か [dx, dy, 形]。</summary>
    private static List<CustomTiles> Custom(params int[][] cells) =>
        [new CustomTileLayer { Cells = cells.ToList() }.ToCustomTiles(100)];

    [Fact]
    public void Draw_CustomTilesStayRelativeToPlayer()
    {
        // 自分で描いたマスはキャラクターのいるマスからの相対位置。キャラクターがどこにいても画面上の同じ位置に描く
        var color = CustomColor;
        var custom = Custom([1, 0]);
        foreach (var player in new[] { new GameCoordinate(62, 5), new GameCoordinate(10, 80) })
        {
            var scene = Scene(null) with { Grid = new IsoGrid(new PointF(400, 300), player), Custom = custom };
            using var bitmap = Render(scene);
            Assert.Equal(color.ToArgb(), bitmap.GetPixel(400 + 32, 300 + 16).ToArgb()); // 右下の隣
            Assert.Equal(Background.ToArgb(), bitmap.GetPixel(400, 300).ToArgb()); // 足元には描かない
        }
    }

    [Fact]
    public void Draw_CustomTilesAboveTerrainAndSkipExcludedRegion()
    {
        var color = CustomColor;
        var custom = Custom([0, 0], [0, -2]);
        using var bitmap = Render(Scene(Blocks, excluded: new Rectangle(380, 290, 40, 20)) with { Custom = custom });
        Assert.Equal(Background.ToArgb(), bitmap.GetPixel(400, 300).ToArgb()); // 除外範囲には描かない
        Assert.Equal(color.ToArgb(), bitmap.GetPixel(400 + 64, 300 - 32).ToArgb()); // (0, -2) はモンスター境界 (62, 3) の上に描く
    }

    [Fact]
    public void Scene_ComparesCustomTilesByContent()
    {
        var custom = Custom([1, 0]);
        Assert.Equal(Scene(Blocks) with { Custom = custom }, Scene(Blocks) with { Custom = custom.ToList() });
        Assert.NotEqual(Scene(Blocks) with { Custom = custom }, Scene(Blocks));
    }

    [Fact]
    public void Draw_CustomShapeFillsOnlyItsParts()
    {
        // 縦 (上・真ん中・下の 1/3 マス) は、マスの左右の 1/3 を塗らない
        var vertical = CustomTileShape.All.Single(shape => shape.Name == "縦").Mask;
        using var bitmap = Render(Scene(null) with { PlayerColor = null, Custom = Custom([0, 0, vertical]) });
        Assert.Equal(CustomColor.ToArgb(), bitmap.GetPixel(400, 300).ToArgb()); // 真ん中
        Assert.Equal(CustomColor.ToArgb(), bitmap.GetPixel(411, 295).ToArgb()); // 上 (画面では右上) の 1/3
        Assert.Equal(Background.ToArgb(), bitmap.GetPixel(389, 295).ToArgb()); // 左 (画面では左上) の 1/3
    }

    [Fact]
    public void Draw_CustomFrameOutlinesTile()
    {
        // 枠はプレイヤーの枠と同じく縁の線だけを描く
        using var bitmap = Render(Scene(null) with { Custom = Custom([1, 0, CustomTileGroup.FrameFlag]) });
        Assert.Equal(CustomColor.ToArgb(), bitmap.GetPixel(448, 308).ToArgb()); // (1, 0) の右上の辺
        Assert.Equal(Background.ToArgb(), bitmap.GetPixel(432, 316).ToArgb()); // 中は塗らない
    }

    [Fact]
    public void Draw_SkipsExcludedRegion()
    {
        // マップ名欄・座標欄の上には描かない (OCR の邪魔になるため)
        using var bitmap = Render(Scene(Blocks, excluded: new Rectangle(380, 290, 40, 20)));
        Assert.Equal(Background.ToArgb(), bitmap.GetPixel(400, 300).ToArgb());
        Assert.Equal(OverlayLayer.MonsterBlock.DefaultColor.ToArgb(), bitmap.GetPixel(400 + 64, 300 - 32).ToArgb()); // (62, 3) は範囲外なので描く
    }
}
