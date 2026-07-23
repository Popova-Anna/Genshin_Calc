namespace GenshinAccountAnalyzer.Calculator.Configuration;

/// <summary>
/// The per-character-level base coefficient used by transformative and additive (Aggravate/Spread)
/// reactions. Values are fixed game data (index 0 = level 1); level 90 equals 1446.8535.
/// </summary>
public static class ReactionLevelTable
{
    private static readonly double[] LevelBase =
    {
        17.165606d, 18.535048d, 19.904854d, 21.274902d, 22.6454d, 24.649612d,
        26.640642d, 28.868587d, 31.36768d, 34.143345d, 37.201d, 40.66d,
        44.446667d, 48.56352d, 53.74848d, 59.081898d, 64.420044d, 69.72446d,
        75.12314d, 80.58478d, 86.11203d, 91.70374d, 97.24463d, 102.812645d,
        108.40956d, 113.20169d, 118.102905d, 122.97932d, 129.72733d, 136.29291d,
        142.67085d, 149.02902d, 155.41699d, 161.8255d, 169.10631d, 176.51808d,
        184.07274d, 191.70952d, 199.55692d, 207.38205d, 215.3989d, 224.16566d,
        233.50217d, 243.35057d, 256.06308d, 268.5435d, 281.52606d, 295.01364d,
        309.0672d, 323.6016d, 336.75754d, 350.5303d, 364.4827d, 378.61917d,
        398.6004d, 416.39825d, 434.387d, 452.95105d, 472.60623d, 492.8849d,
        513.56854d, 539.1032d, 565.51056d, 592.53876d, 624.4434d, 651.47015d,
        679.4968d, 707.79407d, 736.67145d, 765.64026d, 794.7734d, 824.67737d,
        851.1578d, 877.74207d, 914.2291d, 946.74677d, 979.4114d, 1011.223d,
        1044.7917d, 1077.4437d, 1109.9976d, 1142.9766d, 1176.3695d, 1210.1844d,
        1253.8357d, 1288.9528d, 1325.4841d, 1363.4569d, 1405.0974d, 1446.8535d,
    };

    /// <summary>Gets the reaction base coefficient for a character level (1-90, clamped).</summary>
    /// <param name="level">The character level.</param>
    public static double ForLevel(int level)
    {
        int index = Math.Clamp(level, 1, LevelBase.Length) - 1;
        return LevelBase[index];
    }
}
