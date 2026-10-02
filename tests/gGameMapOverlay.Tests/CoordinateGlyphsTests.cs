using gGameMapOverlay.Imaging;
using gGameMapOverlay.Ocr;

namespace gGameMapOverlay.Tests;

public class CoordinateGlyphsTests
{
    private static BgrImage Coordinates(string file) =>
        ScreenshotCase.Get(file).LoadClient().Crop(ScreenshotTests.CoordinateRegion);

    private static string Text(string file) => $"{ScreenshotCase.Get(file).X}:{ScreenshotCase.Get(file).Y}";

    [Theory]
    [MemberData(nameof(ScreenshotCase.Files), MemberType = typeof(ScreenshotCase))]
    public void SplitsOneImagePerCharacter(string file)
    {
        Assert.Equal(Text(file).Length, CoordinateGlyphs.Split(Coordinates(file)).Count);
    }

    [Fact]
    public void ReadsLearnedCharactersInOtherScreenshots()
    {
        var glyphs = new CoordinateGlyphs();
        Assert.Null(glyphs.Recognize(Coordinates("maximized_1920x1080.png")));

        glyphs.Learn(Coordinates("maximized_1920x1080.png"), "40:40");

        Assert.Equal("40:40", glyphs.Recognize(Coordinates("window_1056x660.png")));
        // 9 と 2 はまだ覚えていない。
        Assert.Null(glyphs.Recognize(Coordinates("window_1696x899.png")));
    }

    [Fact]
    public void IgnoresOcrTextThatDoesNotMatchTheImage()
    {
        var glyphs = new CoordinateGlyphs();
        glyphs.Learn(Coordinates("maximized_1920x1080.png"), "40140");
        glyphs.Learn(Coordinates("maximized_1920x1080.png"), "4:40");
        Assert.Equal(0, glyphs.Count);
    }

    [Fact]
    public void ForgetsImagesLearnedAsDifferentCharacters()
    {
        var glyphs = new CoordinateGlyphs();
        glyphs.Learn(Coordinates("maximized_1920x1080.png"), "40:40");
        glyphs.Learn(Coordinates("maximized_1920x1080.png"), "41:41");
        Assert.Null(glyphs.Recognize(Coordinates("maximized_1920x1080.png")));
    }
}
