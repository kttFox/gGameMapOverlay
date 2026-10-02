using gGameMapOverlay.Parsing;

namespace gGameMapOverlay.Tests;

public class TextParsingTests
{
    [Theory]
    [InlineData("123, 456", 123, 456)]
    [InlineData("X:1024 Y:77", 1024, 77)]
    [InlineData("305 , 88", 305, 88)]
    [InlineData("１２３，４５６", 123, 456)] // 全角
    [InlineData("1O3, 4l6", 103, 416)] // 数字に紛れた O / l
    [InlineData("(12:34)", 12, 34)]
    public void ParseCoordinates_ExtractsFirstTwoNumbers(string text, int x, int y)
    {
        Assert.Equal(new GameCoordinate(x, y), TextParsing.ParseCoordinates(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("Lol, Iol")] // 数字を含まない単語は置換しない
    public void ParseCoordinates_ReturnsNullWithoutTwoNumbers(string? text)
    {
        Assert.Null(TextParsing.ParseCoordinates(text));
    }

    [Fact]
    public void CleanMapName_NormalizesWidthAndWhitespace()
    {
        Assert.Equal("レゲ地下洞窟 9F", TextParsing.CleanMapName("  レゲ地下洞窟　 ９Ｆ "));
    }

    [Fact]
    public void CanonicalizeForMatch_DropsSymbolsAndFoldsLookalikes()
    {
        Assert.Equal("rege1ab0", TextParsing.CanonicalizeForMatch("Rege-Lab O"));
        Assert.Equal(TextParsing.CanonicalizeForMatch("레게 지하동굴 9층"), TextParsing.CanonicalizeForMatch("레게지하동글9충"));
    }
}
