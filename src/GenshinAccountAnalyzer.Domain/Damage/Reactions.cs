namespace GenshinAccountAnalyzer.Domain.Damage;

/// <summary>Amplifying reactions, which multiply a hit's damage.</summary>
public enum AmplifyingReaction
{
    /// <summary>No amplifying reaction.</summary>
    None = 0,

    /// <summary>Vaporize (Pyro + Hydro).</summary>
    Vaporize,

    /// <summary>Melt (Pyro + Cryo).</summary>
    Melt
}

/// <summary>Additive (catalyze) reactions, which add flat damage to a hit's base damage.</summary>
public enum AdditiveReaction
{
    /// <summary>No additive reaction.</summary>
    None = 0,

    /// <summary>Aggravate (Electro on Quickened aura).</summary>
    Aggravate,

    /// <summary>Spread (Dendro on Quickened aura).</summary>
    Spread
}

/// <summary>
/// Transformative reactions, which deal their own level-scaled damage. Covers every damage-dealing
/// transformative reaction in the game, including the Lunar variants (which replace Electro-Charged /
/// Bloom when a party member has a Moonsign Benediction-style passive). <see cref="Crystallize"/> is
/// included for completeness but always resolves to zero damage — it produces a shield, not a hit.
/// Lunar-Bloom/-Hyperbloom/-Burgeon are intentionally not separate members: verified against the
/// reference simulator, they spawn the same Dendro Core and reuse <see cref="Bloom"/>/
/// <see cref="Hyperbloom"/>/<see cref="Burgeon"/>'s damage — the "Lunar" flag only changes which
/// team-wide buff event fires, not the hit itself.
/// </summary>
public enum TransformativeReaction
{
    /// <summary>Overloaded (Pyro + Electro).</summary>
    Overloaded = 0,

    /// <summary>Superconduct (Cryo + Electro).</summary>
    Superconduct,

    /// <summary>Electro-Charged (Hydro + Electro).</summary>
    ElectroCharged,

    /// <summary>Swirl (Anemo + element).</summary>
    Swirl,

    /// <summary>Shatter (Blunt/heavy attack on a Frozen target).</summary>
    Shatter,

    /// <summary>Burning (Pyro + Dendro).</summary>
    Burning,

    /// <summary>Bloom (Hydro + Dendro), the seed's burst. Also covers Lunar-Bloom (identical damage).</summary>
    Bloom,

    /// <summary>Hyperbloom (Electro on Dendro Core). Also covers the Lunar-Bloom variant.</summary>
    Hyperbloom,

    /// <summary>Burgeon (Pyro on Dendro Core). Also covers the Lunar-Bloom variant.</summary>
    Burgeon,

    /// <summary>
    /// Crystallize (Geo + element): produces a shield, not damage. Included so the reaction set is
    /// complete; always resolves to zero.
    /// </summary>
    Crystallize,

    /// <summary>
    /// Lunar-Charged: the Lunar variant of Electro-Charged (Hydro + Electro with a Moonsign
    /// Benediction-style passive active). Uses a higher base multiplier and a different EM coefficient
    /// than standard Electro-Charged.
    /// </summary>
    LunarCharged,

    /// <summary>
    /// Lunar-Crystallize: a bonus proc that can occur when Anemo/Geo interacts with an active
    /// Lunar-Charged cloud. Niche relative to the other reactions here, included for completeness.
    /// </summary>
    LunarCrystallize
}
