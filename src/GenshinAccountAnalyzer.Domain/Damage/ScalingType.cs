namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>Which base stat a talent's damage scales from.</summary>
public enum ScalingType
{
    /// <summary>Scales from total ATK.</summary>
    Atk = 0,

    /// <summary>Scales from total HP.</summary>
    Hp,

    /// <summary>Scales from total DEF.</summary>
    Def
}
