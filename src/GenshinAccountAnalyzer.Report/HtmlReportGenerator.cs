using System.Globalization;
using System.Net;
using System.Text;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Report;

/// <summary>
/// Renders an <see cref="AccountAnalysis"/> as a self-contained, styled HTML document (inline CSS, inline
/// SVG/CSS charts, no external dependencies) covering every report section.
/// </summary>
public sealed class HtmlReportGenerator : IReportGenerator
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static readonly (string Id, string Label)[] Sections =
    [
        ("home", "Home"), ("characters", "Characters"), ("weapons", "Weapons"),
        ("artifacts", "Artifacts"), ("teams", "Teams"), ("statistics", "Statistics"),
        ("rating", "Rating"), ("recommendations", "Recommendations"), ("history", "History"),
    ];

    /// <inheritdoc />
    public string GenerateHtml(AccountAnalysis analysis)
    {
        ArgumentNullException.ThrowIfNull(analysis);

        var sb = new StringBuilder(64 * 1024);
        sb.Append("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\">")
          .Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">")
          .Append("<title>Genshin Account ").Append(Enc(analysis.Uid)).Append(" — Report</title>")
          .Append("<style>").Append(ReportTheme.Css).Append("</style></head><body>");

        Hero(sb, analysis);
        Nav(sb);
        sb.Append("<main>");
        Home(sb, analysis);
        Characters(sb, analysis);
        Weapons(sb, analysis);
        Artifacts(sb, analysis);
        Teams(sb, analysis);
        Statistics(sb, analysis);
        Rating(sb, analysis);
        Recommendations(sb, analysis);
        History(sb);
        sb.Append("</main>");

        sb.Append("<footer>Generated ")
          .Append(DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'", Inv))
          .Append(" · Genshin Account Analyzer</footer></body></html>");

        return sb.ToString();
    }

    private static void Hero(StringBuilder sb, AccountAnalysis a)
    {
        int strong = a.Characters.Count(c => c.BuildRating.Tier >= RatingTier.S);
        double topTeam = a.Teams.Count > 0 ? a.Teams[0].Score : 0d;

        sb.Append("<header class=\"hero\"><div class=\"wrap hero-grid\">")
          .Append("<div class=\"gauge\" style=\"--v:").Append(F(a.AverageBuildScore, 0)).Append("\"><div><b>")
          .Append(F(a.AverageBuildScore, 0)).Append("</b><span>AVG BUILD</span></div></div>")
          .Append("<div><h1>Genshin Account ").Append(Enc(a.Uid)).Append("</h1>")
          .Append("<div class=\"sub\">Account analysis report</div><div class=\"tiles\">")
          .Append(Tile(a.Characters.Count.ToString(Inv), "Characters"))
          .Append(Tile(F(a.AverageBuildScore, 1), "Avg build score"))
          .Append(Tile(strong.ToString(Inv), "S-tier or better"))
          .Append(Tile(F(topTeam, 0), "Top team score"))
          .Append("</div></div></div></header>");
    }

    private static void Nav(StringBuilder sb)
    {
        sb.Append("<nav><div class=\"wrap\">");
        foreach ((string id, string label) in Sections)
        {
            sb.Append("<a href=\"#").Append(id).Append("\">").Append(label).Append("</a>");
        }

        sb.Append("</div></nav>");
    }

    private static void Home(StringBuilder sb, AccountAnalysis a)
    {
        double avgEff = a.Characters.Count > 0 ? a.Characters.Average(c => c.Efficiency) : 0d;
        sb.Append(SectionOpen("home", "Overview"))
          .Append("<div class=\"grid\">")
          .Append("<div class=\"card\"><div class=\"meta\">Roster</div><b style=\"font-size:22px\">")
          .Append(a.Characters.Count).Append(" characters</b><div class=\"sub\">Average build efficiency ")
          .Append(F(avgEff, 0)).Append("%</div></div>")
          .Append("<div class=\"card\"><div class=\"meta\">Best team</div><b style=\"font-size:18px\">")
          .Append(a.Teams.Count > 0 ? Enc(a.Teams[0].ReactionCore) : "—")
          .Append("</b><div class=\"sub\">")
          .Append(a.Teams.Count > 0 ? Enc(string.Join(", ", a.Teams[0].Members.Select(m => m.Name))) : "No teams")
          .Append("</div></div>")
          .Append("<div class=\"card\"><div class=\"meta\">Top character</div><b style=\"font-size:18px\">")
          .Append(TopCharacterName(a)).Append("</b><div class=\"sub\">Highest overall build score</div></div>")
          .Append("</div>").Append(SectionClose());
    }

    private static void Characters(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("characters", "Characters")).Append("<div class=\"grid\">");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            string col = ReportTheme.ElementColor(c.Element);
            sb.Append("<div class=\"card\"><div class=\"card-h\"><div><span class=\"dot\" style=\"display:inline-block;background:")
              .Append(col).Append("\"></span> <span class=\"name\">").Append(Enc(c.Name)).Append("</span>")
              .Append("<div class=\"meta\">Lv ").Append(c.Level).Append(" · C").Append(c.ConstellationLevel)
              .Append(" · ").Append(c.Element).Append("</div></div>")
              .Append(TierBadge(c.BuildRating)).Append("</div>")
              .Append(RatingRow("Talents", c.TalentRating))
              .Append(RatingRow("Weapon", c.WeaponRating))
              .Append(RatingRow("Artifacts", c.ArtifactRating))
              .Append("<div class=\"statrow\">")
              .Append(Kv("CR/CD", F(c.CritBalance.CritRate, 0) + "/" + F(c.CritBalance.CritDamage, 0)))
              .Append(Kv("ER", F(c.EnergyRecharge * 100, 0) + "%"))
              .Append(Kv("EM", F(c.ElementalMastery, 0)))
              .Append(Kv("CV", F(c.CritBalance.CritValue, 0)))
              .Append("</div><div class=\"tags\">");

            foreach (string s in c.Strengths.Take(2))
            {
                sb.Append("<span class=\"chip good\">").Append(Enc(Trim(s))).Append("</span>");
            }

            foreach (string w in c.Weaknesses.Take(2))
            {
                sb.Append("<span class=\"chip bad\">").Append(Enc(Trim(w))).Append("</span>");
            }

            sb.Append("</div></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void Weapons(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("weapons", "Weapons"))
          .Append("<table><thead><tr><th>Character</th><th>Equipped</th><th>DPS loss vs BiS</th><th>Suggested best-in-slot</th></tr></thead><tbody>");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            string equipped = c.Weapon?.Equipped?.Name ?? "—";
            string bis = c.BestWeapon?.Name ?? "—";
            double loss = c.Weapon?.DpsLossVsBis ?? 0d;
            sb.Append("<tr><td>").Append(Enc(c.Name)).Append("</td><td>").Append(Enc(equipped))
              .Append("</td><td>").Append(F(loss, 0)).Append("%</td><td>").Append(Enc(bis)).Append("</td></tr>");
        }

        sb.Append("</tbody></table>").Append(SectionClose());
    }

    private static void Artifacts(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("artifacts", "Artifacts"))
          .Append("<table><thead><tr><th>Character</th><th>Sets</th><th>Avg efficiency</th><th>Total CV</th><th>Dead rolls</th><th>Goblet rec.</th></tr></thead><tbody>");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            if (c.Artifacts.Count == 0)
            {
                continue;
            }

            double avgEff = c.Artifacts.Average(x => x.Efficiency);
            double totalCv = c.Artifacts.Sum(x => x.CritValue);
            double dead = c.Artifacts.Sum(x => x.DeadRolls);
            string sets = c.BestArtifacts is { CurrentSets.Count: > 0 } br
                ? string.Join(", ", br.CurrentSets.Select(s => $"{s.SetName} ({s.PieceCount})"))
                : "—";
            string goblet = c.BestArtifacts is { } ba && ba.MainStats.TryGetValue(ArtifactSlot.Goblet, out StatType g)
                ? g.ToString()
                : "—";
            sb.Append("<tr><td>").Append(Enc(c.Name)).Append("</td><td>").Append(Enc(sets))
              .Append("</td><td>").Append(F(avgEff, 0)).Append("%</td><td>").Append(F(totalCv, 0))
              .Append("</td><td>").Append(F(dead, 1)).Append("</td><td>").Append(Enc(goblet)).Append("</td></tr>");
        }

        sb.Append("</tbody></table>").Append(SectionClose());
    }

    private static void Teams(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("teams", "Best teams")).Append("<div class=\"grid\">");
        int rank = 1;
        foreach (TeamAnalysis t in a.Teams.Take(6))
        {
            sb.Append("<div class=\"card\"><div class=\"card-h\"><span class=\"name\">#").Append(rank++)
              .Append(" ").Append(Enc(t.ReactionCore)).Append("</span><span class=\"badge\" style=\"background:#7c8cff\">")
              .Append(F(t.Score, 0)).Append("</span></div><div class=\"tags\">");
            foreach (TeamResonance r in t.Resonances)
            {
                sb.Append("<span class=\"chip\">").Append(Enc(r.Name)).Append("</span>");
            }

            sb.Append("</div><div class=\"members\">");
            foreach (TeamMember m in t.Members)
            {
                sb.Append("<div class=\"member\"><span class=\"dot\" style=\"background:")
                  .Append(ReportTheme.ElementColor(m.Element)).Append("\"></span>").Append(Enc(m.Name))
                  .Append("<span class=\"role\">").Append(Enc(m.Role)).Append("</span></div>");
            }

            sb.Append("</div><div class=\"meta\">").Append(Enc(string.Join(" · ", t.Reasons))).Append("</div></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void Statistics(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("statistics", "Statistics")).Append("<div class=\"grid\">");

        // Element distribution.
        var byElement = a.Characters.GroupBy(c => c.Element)
            .Select(g => (Element: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();
        int maxEl = byElement.Count > 0 ? byElement.Max(x => x.Count) : 1;
        sb.Append("<div class=\"card\"><div class=\"meta\" style=\"margin-bottom:8px\">Element distribution</div>");
        foreach ((ElementType element, int count) in byElement)
        {
            sb.Append("<div class=\"distrow\"><span class=\"lbl\">").Append(element).Append("</span>")
              .Append("<span class=\"bar\"><i style=\"width:").Append(F(count * 100.0 / maxEl, 0))
              .Append("%;background:").Append(ReportTheme.ElementColor(element)).Append("\"></i></span><b>")
              .Append(count).Append("</b></div>");
        }

        sb.Append("</div>");

        // Tier distribution.
        var byTier = a.Characters.GroupBy(c => c.BuildRating.Tier)
            .Select(g => (Tier: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Tier)
            .ToList();
        int maxTier = byTier.Count > 0 ? byTier.Max(x => x.Count) : 1;
        sb.Append("<div class=\"card\"><div class=\"meta\" style=\"margin-bottom:8px\">Build tier distribution</div>");
        foreach ((RatingTier tier, int count) in byTier)
        {
            sb.Append("<div class=\"distrow\"><span class=\"lbl\">").Append(tier).Append("</span>")
              .Append("<span class=\"bar\"><i style=\"width:").Append(F(count * 100.0 / maxTier, 0))
              .Append("%;background:").Append(ReportTheme.TierColor(tier)).Append("\"></i></span><b>")
              .Append(count).Append("</b></div>");
        }

        sb.Append("</div></div>").Append(SectionClose());
    }

    private static void Rating(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("rating", "Character ranking")).Append("<div class=\"card\">");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            sb.Append("<div class=\"rowlbl\"><span>").Append(Enc(c.Name)).Append("</span><span>")
              .Append(F(c.OverallScore, 0)).Append(" · ").Append(c.BuildRating.Tier).Append("</span></div>")
              .Append("<div class=\"bar\"><i style=\"width:").Append(F(c.OverallScore, 0))
              .Append("%;background:").Append(ReportTheme.TierColor(c.BuildRating.Tier)).Append("\"></i></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void Recommendations(StringBuilder sb, AccountAnalysis a)
    {
        var all = a.Characters
            .SelectMany(c => c.Recommendations.Select(r => (Character: c.Name, Rec: r)))
            .OrderByDescending(x => x.Rec.Priority)
            .Take(30)
            .ToList();

        sb.Append(SectionOpen("recommendations", "Recommendations")).Append("<div class=\"card\">");
        if (all.Count == 0)
        {
            sb.Append("<div class=\"sub\">No recommendations — every build looks solid.</div>");
        }

        foreach ((string character, Recommendation rec) in all)
        {
            sb.Append("<div class=\"rec\"><span class=\"prio\" style=\"background:")
              .Append(ReportTheme.PriorityColor(rec.Priority)).Append("\">").Append(rec.Priority.ToString().ToUpperInvariant())
              .Append("</span><div><b>").Append(Enc(character)).Append(" — ").Append(Enc(rec.Title))
              .Append("</b><div class=\"sub\">").Append(Enc(rec.Detail)).Append("</div></div></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void History(StringBuilder sb)
    {
        sb.Append(SectionOpen("history", "Change history")).Append("<div class=\"card\"><div class=\"sub\">")
          .Append("Snapshot generated ").Append(DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'", Inv))
          .Append(". Historical comparison across snapshots arrives in a later iteration.")
          .Append("</div></div>").Append(SectionClose());
    }

    // --- helpers ---

    private static string SectionOpen(string id, string title) =>
        $"<section id=\"{id}\"><div class=\"wrap\"><h2>{Enc(title)}</h2>";

    private static string SectionClose() => "</div></section>";

    private static string Tile(string value, string label) =>
        $"<div class=\"tile\"><b>{Enc(value)}</b><span>{Enc(label)}</span></div>";

    private static string Kv(string label, string value) =>
        $"<span class=\"kv\">{Enc(label)} <b>{Enc(value)}</b></span>";

    private static string RatingRow(string label, Rating rating)
    {
        string color = ReportTheme.TierColor(rating.Tier);
        return $"<div class=\"rowlbl\"><span>{Enc(label)}</span><span>{F(rating.Score, 0)}</span></div>"
            + $"<div class=\"bar\"><i style=\"width:{F(rating.Score, 0)}%;background:{color}\"></i></div>";
    }

    private static string TierBadge(Rating rating) =>
        $"<span class=\"badge\" style=\"background:{ReportTheme.TierColor(rating.Tier)}\">{rating.Tier} {F(rating.Score, 0)}</span>";

    private static string TopCharacterName(AccountAnalysis a) =>
        a.Characters.Count == 0 ? "—" : Enc(a.Characters.MaxBy(c => c.OverallScore)!.Name);

    private static string Trim(string text) => text.Length <= 42 ? text : text[..40] + "…";

    private static string Enc(string? text) => WebUtility.HtmlEncode(text ?? string.Empty);

    private static string F(double value, int digits) =>
        value.ToString("F" + digits.ToString(Inv), Inv);
}
