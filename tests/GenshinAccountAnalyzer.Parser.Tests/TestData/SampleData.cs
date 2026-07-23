namespace GenshinAccountAnalyzer.Parser.Tests.TestData;

/// <summary>Helpers for locating and opening the bundled Enka sample export.</summary>
internal static class SampleData
{
    /// <summary>The UID of the bundled sample account.</summary>
    public const string SampleUid = "627846506";

    /// <summary>Avatar id of the first character in the sample (Kaedehara Kazuha).</summary>
    public const int KazuhaAvatarId = 10000047;

    /// <summary>Full path to the bundled Enka sample JSON, copied next to the test assembly.</summary>
    public static string EnkaSamplePath =>
        Path.Combine(AppContext.BaseDirectory, "TestData", "enka-sample.json");

    /// <summary>Opens the bundled Enka sample as a readable stream.</summary>
    /// <returns>A file stream positioned at the start of the sample.</returns>
    public static FileStream OpenEnkaSample() =>
        new(EnkaSamplePath, FileMode.Open, FileAccess.Read, FileShare.Read);
}
