using gGameMapOverlay.Overlay;

namespace gGameMapOverlay.Tests;

public class SlideAccuracyTests
{
    [Fact]
    public void Expected_MovesLinearlyFromStepStart()
    {
        var accuracy = new SlideAccuracy();
        accuracy.AddStep(100, (5, 5), (6, 5));
        accuracy.AddStep(300, (6, 5), (6, 6));
        Assert.Equal((5.0, 5.0), accuracy.Expected(50, 200));   // 歩き出す前は最初のマス
        Assert.Equal((5.5, 5.0), accuracy.Expected(200, 200));  // 1 歩の半分
        Assert.Equal((6.0, 5.0), accuracy.Expected(299, 200) is var (x, y) ? (Math.Round(x, 2), y) : default);
        Assert.Equal((6.0, 6.0), accuracy.Expected(600, 200));  // 着いたら止まる
    }

    [Fact]
    public void Summarize_OnTime_HasNoError()
    {
        var accuracy = new SlideAccuracy();
        accuracy.AddStep(0, (0, 0), (1, 0));
        for (var t = 0; t <= 200; t += 20)
        {
            accuracy.AddFrame(t, t / 200.0, 0);
        }
        var result = accuracy.Summarize(200)!.Value;
        Assert.Equal(0, result.Max, 6);
        Assert.Equal(0, result.LeadMs, 6);
    }

    /// <summary>20 ms 先に描いていれば、進みは +20 ms。</summary>
    [Fact]
    public void Summarize_Ahead_ReportsLead()
    {
        var accuracy = new SlideAccuracy();
        accuracy.AddStep(0, (0, 0), (0, -1));
        for (var t = 0; t <= 160; t += 20)
        {
            accuracy.AddFrame(t, 0, -(t + 20) / 200.0);
        }
        var result = accuracy.Summarize(200)!.Value;
        Assert.Equal(20, result.LeadMs, 6);
        Assert.Equal(0.1, result.Max, 6);
        Assert.Equal(1, result.MaxStep);
    }

    [Fact]
    public void Summarize_NoSteps_IsNull()
    {
        var accuracy = new SlideAccuracy();
        accuracy.AddFrame(0, 1, 1);
        Assert.Null(accuracy.Summarize(200));
    }
}
