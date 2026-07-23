using FluentAssertions;
using GenshinAccountAnalyzer.Calculator.Configuration;

namespace GenshinAccountAnalyzer.Calculator.Tests;

public sealed class DamageFormulaTests
{
    [Fact]
    public void DefenseMultiplier_EqualLevels_IsOneHalf()
    {
        DamageFormula.DefenseMultiplier(90, 90, 0, 0).Should().BeApproximately(0.5, 1e-9);
    }

    [Fact]
    public void DefenseMultiplier_WithDefenseShred_IsHigher()
    {
        // 190 / (190 + 190 * 0.8) = 190 / 342.
        DamageFormula.DefenseMultiplier(90, 90, 0.2, 0).Should().BeApproximately(190d / 342d, 1e-9);
    }

    [Theory]
    [InlineData(0.1, 0.9)]           // normal: 1 - res
    [InlineData(0.0, 1.0)]
    [InlineData(-0.2, 1.1)]          // negative: 1 - res/2
    [InlineData(0.8, 1d / 4.2)]      // high: 1 / (4*res + 1)
    public void ResistanceMultiplier_MatchesPiecewiseFormula(double resistance, double expected)
    {
        DamageFormula.ResistanceMultiplier(resistance).Should().BeApproximately(expected, 1e-9);
    }

    [Fact]
    public void ReactionLevelTable_Level90_IsCanonicalValue()
    {
        ReactionLevelTable.ForLevel(90).Should().BeApproximately(1446.8535, 1e-4);
    }

    [Fact]
    public void ReactionLevelTable_ClampsOutOfRange()
    {
        ReactionLevelTable.ForLevel(0).Should().Be(ReactionLevelTable.ForLevel(1));
        ReactionLevelTable.ForLevel(999).Should().Be(ReactionLevelTable.ForLevel(90));
    }
}
