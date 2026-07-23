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

/// <summary>Transformative reactions, which deal their own level-scaled damage.</summary>
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

    /// <summary>Shatter (Blunt/heavy attack on Frozen).</summary>
    Shatter,

    /// <summary>Burning (Pyro + Dendro).</summary>
    Burning,

    /// <summary>Bloom (Hydro + Dendro), the seed's burst.</summary>
    Bloom,

    /// <summary>Hyperbloom (Electro on Dendro Core).</summary>
    Hyperbloom,

    /// <summary>Burgeon (Pyro on Dendro Core).</summary>
    Burgeon
}
