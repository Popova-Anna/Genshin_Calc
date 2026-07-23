using FluentAssertions;
using GenshinAccountAnalyzer.Analyzer.Configuration;
using GenshinAccountAnalyzer.Domain.Analysis;

namespace GenshinAccountAnalyzer.Analyzer.Tests.Configuration;

public sealed class RatingScaleTests
{
    [Theory]
    [InlineData(100, RatingTier.SS)]
    [InlineData(95, RatingTier.SS)]
    [InlineData(94.9, RatingTier.S)]
    [InlineData(85, RatingTier.S)]
    [InlineData(75, RatingTier.A)]
    [InlineData(60, RatingTier.B)]
    [InlineData(45, RatingTier.C)]
    [InlineData(30, RatingTier.D)]
    [InlineData(29.9, RatingTier.F)]
    [InlineData(0, RatingTier.F)]
    public void Classify_MapsScoreToExpectedTier(double score, RatingTier expected)
    {
        RatingScale.Classify(score).Should().Be(expected);
    }

    [Theory]
    [InlineData(150, 100)]
    [InlineData(-10, 0)]
    public void ToRating_ClampsScoreToRange(double raw, double expected)
    {
        RatingScale.ToRating(raw).Score.Should().Be(expected);
    }
}
