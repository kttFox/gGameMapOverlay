namespace gGameMapOverlay.Tests;

public class WalkSpeedTests
{
    [Theory]
    [InlineData(300, 0)]
    [InlineData(286, 0)]
    [InlineData(285, 1)]
    [InlineData(263, 2)]
    [InlineData(256, 3)]
    [InlineData(243, 4)]
    [InlineData(200, 4)]
    public void NearestGrade_PicksClosestStep(double stepMs, int grade) => Assert.Equal(grade, WalkSpeed.NearestGrade(stepMs));

    [Theory]
    [InlineData(0, 294)]
    [InlineData(2, 263)]
    [InlineData(4, 238)]
    public void StepMs_IsTableValue(int grade, int stepMs) => Assert.Equal(stepMs, WalkSpeed.Grade(grade).StepMs);

    /// <summary>1 歩ごとの撮影の揺れ (数 ms) があっても、続けて歩いた平均で等級を当てる。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void AddChange_JitteredWalk_FindsGrade(int grade)
    {
        var speed = new WalkSpeed();
        var step = WalkSpeed.Grade(grade).StepMs;
        double[] jitter = [0, 6, -5, 4, -6, 5, -3, 6, -4];
        for (var i = 0; i < jitter.Length; i++)
        {
            speed.AddChange(1000 + i * step + jitter[i]);
        }
        Assert.Equal(grade, speed.MeasuredGrade);
    }

    [Fact]
    public void AddChange_TooFewSteps_NotMeasured()
    {
        var speed = new WalkSpeed();
        speed.AddChange(0);
        speed.AddChange(263);
        speed.AddChange(526);
        Assert.Null(speed.MeasuredGrade);
        speed.AddChange(789);
        Assert.Equal(2, speed.MeasuredGrade);
    }

    /// <summary>止まって歩き出したときの長い間隔や、スキルの短い間隔は 1 歩に数えない。止まっても最後の判定は残す。</summary>
    [Fact]
    public void AddChange_PausesAndSkills_BreakRun()
    {
        var speed = new WalkSpeed();
        for (var i = 0; i < 5; i++)
        {
            speed.AddChange(i * 238);
        }
        Assert.Equal(4, speed.MeasuredGrade);
        // 止まって、歩き出した: 2 歩だけでは判定を変えない
        speed.AddChange(10_000);
        speed.AddChange(10_294);
        speed.AddChange(10_588);
        Assert.Equal(4, speed.MeasuredGrade);
        // スキル (65 ms ごと) で区切られる
        speed.AddChange(10_653);
        speed.AddChange(10_718);
        Assert.Equal(4, speed.MeasuredGrade);
        for (var i = 1; i <= 3; i++)
        {
            speed.AddChange(10_718 + i * 294);
        }
        Assert.Equal(0, speed.MeasuredGrade);
    }
}
