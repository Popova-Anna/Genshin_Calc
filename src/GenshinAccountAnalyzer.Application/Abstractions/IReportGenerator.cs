using GenshinAccountAnalyzer.Domain.Analysis;

namespace GenshinAccountAnalyzer.Application.Abstractions;

/// <summary>
/// Renders an account analysis into a self-contained report document.
/// </summary>
public interface IReportGenerator
{
    /// <summary>Renders the analysis as a standalone HTML document.</summary>
    /// <param name="analysis">The account analysis to render.</param>
    /// <returns>A complete HTML document as a string.</returns>
    string GenerateHtml(AccountAnalysis analysis);
}
