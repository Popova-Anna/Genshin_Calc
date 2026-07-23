namespace GenshinAccountAnalyzer.Domain.Models;

/// <summary>
/// The player-level profile information for an account.
/// </summary>
public sealed record Profile
{
    /// <summary>The player's nickname.</summary>
    public required string Nickname { get; init; }

    /// <summary>Adventure Rank.</summary>
    public int AdventureRank { get; init; }

    /// <summary>World Level.</summary>
    public int WorldLevel { get; init; }

    /// <summary>Profile signature text, when set.</summary>
    public string? Signature { get; init; }

    /// <summary>Total achievements completed.</summary>
    public int Achievements { get; init; }

    /// <summary>Deepest Spiral Abyss floor reached, when available (e.g. 12).</summary>
    public int? SpiralAbyssFloor { get; init; }

    /// <summary>Deepest Spiral Abyss chamber reached, when available (e.g. 3).</summary>
    public int? SpiralAbyssChamber { get; init; }

    /// <summary>Character id shown as the profile picture, when available.</summary>
    public int? ProfilePictureCharacterId { get; init; }
}
