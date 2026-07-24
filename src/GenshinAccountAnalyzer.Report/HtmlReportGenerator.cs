using System.Globalization;
using System.Net;
using System.Text;
using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Common;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Report;

/// <summary>
/// Renders an <see cref="AccountAnalysis"/> as a self-contained, styled, bilingual (English/Russian)
/// HTML document with an in-page language toggle. Inline CSS/JS and CSS charts — no external code
/// dependencies (character icons load from a remote host with a graceful fallback).
/// </summary>
public sealed class HtmlReportGenerator : IReportGenerator
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static readonly (string Id, string En, string Ru)[] Sections =
    [
        ("home", "Home", "Главная"), ("characters", "Characters", "Персонажи"),
        ("weapons", "Weapons", "Оружие"), ("artifacts", "Artifacts", "Артефакты"),
        ("teams", "Teams", "Пачки"), ("statistics", "Statistics", "Статистика"),
        ("rating", "Rating", "Рейтинг"), ("recommendations", "Recommendations", "Рекомендации"),
        ("history", "History", "История"),
    ];

    /// <inheritdoc />
    public string GenerateHtml(AccountAnalysis analysis)
    {
        ArgumentNullException.ThrowIfNull(analysis);

        var sb = new StringBuilder(96 * 1024);
        sb.Append("<!DOCTYPE html><html lang=\"en\" class=\"lang-en\"><head><meta charset=\"utf-8\">")
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
          .Append(" · Genshin Account Analyzer</footer>")
          .Append("<script>function toggleLang(){var h=document.documentElement,en=h.classList.contains('lang-en');")
          .Append("h.classList.toggle('lang-en',!en);h.classList.toggle('lang-ru',en);")
          .Append("var b=document.getElementById('langbtn');if(b)b.textContent=en?'EN':'RU';}</script>")
          .Append("</body></html>");

        return sb.ToString();
    }

    private static void Hero(StringBuilder sb, AccountAnalysis a)
    {
        int strong = a.Characters.Count(c => c.BuildRating.Tier >= RatingTier.S);
        double topTeam = a.Teams.Count > 0 ? a.Teams[0].Score : 0d;

        sb.Append("<header class=\"hero\"><div class=\"wrap hero-grid\">")
          .Append("<div class=\"gauge\" style=\"--v:").Append(F(a.AverageBuildScore, 0)).Append("\"><div><b>")
          .Append(F(a.AverageBuildScore, 0)).Append("</b><span>")
          .Append(Bi("AVG BUILD", "СР. БИЛД")).Append("</span></div></div>")
          .Append("<div><h1>Genshin Account ").Append(Enc(a.Uid)).Append("</h1>")
          .Append("<div class=\"sub\">").Append(Bi("Account analysis report", "Отчёт анализа аккаунта")).Append("</div><div class=\"tiles\">")
          .Append(Tile(a.Characters.Count.ToString(Inv), "Characters", "Персонажи"))
          .Append(Tile(F(a.AverageBuildScore, 1), "Avg build score", "Ср. счёт билда"))
          .Append(Tile(strong.ToString(Inv), "S-tier or better", "S-тир и выше"))
          .Append(Tile(F(topTeam, 0), "Top team score", "Счёт лучшей пачки"))
          .Append("</div></div></div></header>");
    }

    private static void Nav(StringBuilder sb)
    {
        sb.Append("<nav><div class=\"wrap\">");
        foreach ((string id, string en, string ru) in Sections)
        {
            sb.Append("<a href=\"#").Append(id).Append("\">").Append(Bi(en, ru)).Append("</a>");
        }

        sb.Append("<button id=\"langbtn\" class=\"langbtn\" onclick=\"toggleLang()\">RU</button></div></nav>");
    }

    private static void Home(StringBuilder sb, AccountAnalysis a)
    {
        double avgEff = a.Characters.Count > 0 ? a.Characters.Average(c => c.Efficiency) : 0d;
        sb.Append(SectionOpen("home", "Overview", "Обзор"))
          .Append("<div class=\"grid\">")
          .Append("<div class=\"card\"><div class=\"meta\">").Append(Bi("Roster", "Ростер")).Append("</div><b style=\"font-size:22px\">")
          .Append(a.Characters.Count).Append(" ").Append(Bi("characters", "персонажей")).Append("</b><div class=\"sub\">")
          .Append(Bi("Average build efficiency ", "Средняя эффективность билда ")).Append(F(avgEff, 0)).Append("%</div></div>")
          .Append("<div class=\"card\"><div class=\"meta\">").Append(Bi("Best team", "Лучшая пачка")).Append("</div><b style=\"font-size:18px\">")
          .Append(a.Teams.Count > 0 ? BiT(a.Teams[0].ReactionCore) : "—")
          .Append("</b><div class=\"sub\">")
          .Append(a.Teams.Count > 0 ? MemberNames(a.Teams[0]) : "—")
          .Append("</div></div>")
          .Append("<div class=\"card\"><div class=\"meta\">").Append(Bi("Top character", "Топ-персонаж")).Append("</div><b style=\"font-size:18px\">")
          .Append(TopCharacterName(a)).Append("</b><div class=\"sub\">").Append(Bi("Highest overall build score", "Наивысший счёт билда")).Append("</div></div>")
          .Append("</div>").Append(SectionClose());
    }

    private static void Characters(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("characters", "Characters", "Персонажи")).Append("<div class=\"grid\">");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            string col = ReportTheme.ElementColor(c.Element);
            sb.Append("<div class=\"card\"><div class=\"card-h\"><div class=\"chead\">")
              .Append(Avatar(c, col))
              .Append("<div><span class=\"name\">").Append(Name(c)).Append("</span>")
              .Append("<div class=\"meta\">").Append(Bi("Lv ", "Ур. ")).Append(c.Level).Append(" · C").Append(c.ConstellationLevel)
              .Append(" · ").Append(c.Element).Append("</div></div></div>")
              .Append(TierBadge(c.BuildRating)).Append("</div>")
              .Append(RatingRow("Talents", "Таланты", c.TalentRating))
              .Append(RatingRow("Weapon", "Оружие", c.WeaponRating))
              .Append(RatingRow("Artifacts", "Артефакты", c.ArtifactRating))
              .Append("<div class=\"statrow\">")
              .Append(Kv("CR/CD", F(c.CritBalance.CritRate, 0) + "/" + F(c.CritBalance.CritDamage, 0)))
              .Append(Kv("ER", F(c.EnergyRecharge * 100, 0) + "%"))
              .Append(Kv("EM", F(c.ElementalMastery, 0)))
              .Append(Kv("CV", F(c.CritBalance.CritValue, 0)))
              .Append("</div><div class=\"tags\">");

            foreach (LocalizedText s in c.Strengths.Take(2))
            {
                sb.Append("<span class=\"chip good\">").Append(BiT(s)).Append("</span>");
            }

            foreach (LocalizedText w in c.Weaknesses.Take(2))
            {
                sb.Append("<span class=\"chip bad\">").Append(BiT(w)).Append("</span>");
            }

            sb.Append("</div></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void Weapons(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("weapons", "Weapons", "Оружие"))
          .Append("<table><thead><tr><th>").Append(Bi("Character", "Персонаж"))
          .Append("</th><th>").Append(Bi("Equipped", "Экипировано"))
          .Append("</th><th>").Append(Bi("DPS loss vs BiS", "Потеря DPS от BiS"))
          .Append("</th><th>").Append(Bi("Suggested best-in-slot", "Рекомендуемое BiS")).Append("</th></tr></thead><tbody>");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            sb.Append("<tr><td>").Append(Name(c)).Append("</td><td>").Append(Enc(c.Weapon?.Equipped?.Name ?? "—"))
              .Append("</td><td>").Append(F(c.Weapon?.DpsLossVsBis ?? 0d, 0)).Append("%</td><td>")
              .Append(Enc(c.BestWeapon?.Name ?? "—")).Append("</td></tr>");
        }

        sb.Append("</tbody></table>").Append(SectionClose());
    }

    private static void Artifacts(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("artifacts", "Artifacts", "Артефакты"))
          .Append("<table><thead><tr><th>").Append(Bi("Character", "Персонаж"))
          .Append("</th><th>").Append(Bi("Sets", "Сеты"))
          .Append("</th><th>").Append(Bi("Avg efficiency", "Ср. эффективность"))
          .Append("</th><th>").Append(Bi("Total CV", "Сумма CV"))
          .Append("</th><th>").Append(Bi("Dead rolls", "Мёртвые роллы"))
          .Append("</th><th>").Append(Bi("Goblet rec.", "Кубок")).Append("</th></tr></thead><tbody>");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            if (c.Artifacts.Count == 0)
            {
                continue;
            }

            string sets = c.BestArtifacts is { CurrentSets.Count: > 0 } br
                ? string.Join(", ", br.CurrentSets.Select(s => $"{s.SetName} ({s.PieceCount})"))
                : "—";
            string goblet = c.BestArtifacts is { } ba && ba.MainStats.TryGetValue(ArtifactSlot.Goblet, out StatType g)
                ? g.ToString()
                : "—";
            sb.Append("<tr><td>").Append(Name(c)).Append("</td><td>").Append(Enc(sets))
              .Append("</td><td>").Append(F(c.Artifacts.Average(x => x.Efficiency), 0)).Append("%</td><td>")
              .Append(F(c.Artifacts.Sum(x => x.CritValue), 0)).Append("</td><td>")
              .Append(F(c.Artifacts.Sum(x => x.DeadRolls), 1)).Append("</td><td>").Append(Enc(goblet)).Append("</td></tr>");
        }

        sb.Append("</tbody></table>").Append(SectionClose());
    }

    private static void Teams(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("teams", "Best teams", "Лучшие пачки")).Append("<div class=\"grid\">");
        int rank = 1;
        foreach (TeamAnalysis t in a.Teams.Take(6))
        {
            sb.Append("<div class=\"card\"><div class=\"card-h\"><span class=\"name\">#").Append(rank++)
              .Append(" ").Append(BiT(t.ReactionCore)).Append("</span><span class=\"badge\" style=\"background:#7c8cff\">")
              .Append(F(t.Score, 0)).Append("</span></div><div class=\"tags\">");
            foreach (TeamResonance r in t.Resonances)
            {
                sb.Append("<span class=\"chip\">").Append(BiT(r.Name)).Append("</span>");
            }

            sb.Append("</div><div class=\"members\">");
            foreach (TeamMember m in t.Members)
            {
                sb.Append("<div class=\"member\"><span class=\"dot\" style=\"background:")
                  .Append(ReportTheme.ElementColor(m.Element)).Append("\"></span>")
                  .Append(Bi(m.Name, string.IsNullOrEmpty(m.NameRu) ? m.Name : m.NameRu!))
                  .Append("<span class=\"role\">").Append(RoleLabel(m.Role)).Append("</span></div>");
            }

            sb.Append("</div><div class=\"meta\">")
              .Append(Bi(string.Join(" · ", t.Reasons.Select(r => r.En)), string.Join(" · ", t.Reasons.Select(r => r.Ru))))
              .Append("</div></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void Statistics(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("statistics", "Statistics", "Статистика")).Append("<div class=\"grid\">");

        var byElement = a.Characters.GroupBy(c => c.Element)
            .Select(g => (Element: g.Key, Count: g.Count())).OrderByDescending(x => x.Count).ToList();
        int maxEl = byElement.Count > 0 ? byElement.Max(x => x.Count) : 1;
        sb.Append("<div class=\"card\"><div class=\"meta\" style=\"margin-bottom:8px\">")
          .Append(Bi("Element distribution", "Распределение по стихиям")).Append("</div>");
        foreach ((ElementType element, int count) in byElement)
        {
            sb.Append(DistRow(element.ToString(), count, maxEl, ReportTheme.ElementColor(element)));
        }

        sb.Append("</div><div class=\"card\"><div class=\"meta\" style=\"margin-bottom:8px\">")
          .Append(Bi("Build tier distribution", "Распределение по тирам")).Append("</div>");
        var byTier = a.Characters.GroupBy(c => c.BuildRating.Tier)
            .Select(g => (Tier: g.Key, Count: g.Count())).OrderByDescending(x => x.Tier).ToList();
        int maxTier = byTier.Count > 0 ? byTier.Max(x => x.Count) : 1;
        foreach ((RatingTier tier, int count) in byTier)
        {
            sb.Append(DistRow(tier.ToString(), count, maxTier, ReportTheme.TierColor(tier)));
        }

        sb.Append("</div></div>").Append(SectionClose());
    }

    private static void Rating(StringBuilder sb, AccountAnalysis a)
    {
        sb.Append(SectionOpen("rating", "Character ranking", "Рейтинг персонажей")).Append("<div class=\"card\">");
        foreach (CharacterAnalysis c in a.Characters.OrderByDescending(c => c.OverallScore))
        {
            sb.Append("<div class=\"rowlbl\"><span>").Append(Name(c)).Append("</span><span>")
              .Append(F(c.OverallScore, 0)).Append(" · ").Append(c.BuildRating.Tier).Append("</span></div>")
              .Append("<div class=\"bar\"><i style=\"width:").Append(F(c.OverallScore, 0))
              .Append("%;background:").Append(ReportTheme.TierColor(c.BuildRating.Tier)).Append("\"></i></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void Recommendations(StringBuilder sb, AccountAnalysis a)
    {
        var all = a.Characters
            .SelectMany(c => c.Recommendations.Select(r => (Character: c, Rec: r)))
            .OrderByDescending(x => x.Rec.Priority)
            .Take(30)
            .ToList();

        sb.Append(SectionOpen("recommendations", "Recommendations", "Рекомендации")).Append("<div class=\"card\">");
        if (all.Count == 0)
        {
            sb.Append("<div class=\"sub\">").Append(Bi("No recommendations — every build looks solid.", "Рекомендаций нет — все билды в порядке.")).Append("</div>");
        }

        foreach ((CharacterAnalysis character, Recommendation rec) in all)
        {
            sb.Append("<div class=\"rec\"><span class=\"prio\" style=\"background:")
              .Append(ReportTheme.PriorityColor(rec.Priority)).Append("\">").Append(PriorityLabel(rec.Priority))
              .Append("</span><div><b>").Append(Name(character)).Append(" — ").Append(BiT(rec.Title))
              .Append("</b><div class=\"sub\">").Append(BiT(rec.Detail)).Append("</div></div></div>");
        }

        sb.Append("</div>").Append(SectionClose());
    }

    private static void History(StringBuilder sb)
    {
        sb.Append(SectionOpen("history", "Change history", "История изменений")).Append("<div class=\"card\"><div class=\"sub\">")
          .Append(Bi(
              $"Snapshot generated {DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'", Inv)}. Historical comparison across snapshots arrives in a later iteration.",
              $"Снимок создан {DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'", Inv)}. Сравнение снимков во времени появится в следующей итерации."))
          .Append("</div></div>").Append(SectionClose());
    }

    // --- helpers ---

    private static string SectionOpen(string id, string en, string ru) =>
        $"<section id=\"{id}\"><div class=\"wrap\"><h2>{Bi(en, ru)}</h2>";

    private static string SectionClose() => "</div></section>";

    private static string Tile(string value, string en, string ru) =>
        $"<div class=\"tile\"><b>{Enc(value)}</b><span>{Bi(en, ru)}</span></div>";

    private static string Kv(string label, string value) =>
        $"<span class=\"kv\">{Enc(label)} <b>{Enc(value)}</b></span>";

    private static string DistRow(string label, int count, int max, string color) =>
        $"<div class=\"distrow\"><span class=\"lbl\">{Enc(label)}</span>"
        + $"<span class=\"bar\"><i style=\"width:{F(count * 100.0 / max, 0)}%;background:{color}\"></i></span><b>{count}</b></div>";

    private static string RatingRow(string en, string ru, Rating rating)
    {
        string color = ReportTheme.TierColor(rating.Tier);
        return $"<div class=\"rowlbl\"><span>{Bi(en, ru)}</span><span>{F(rating.Score, 0)}</span></div>"
            + $"<div class=\"bar\"><i style=\"width:{F(rating.Score, 0)}%;background:{color}\"></i></div>";
    }

    private static string TierBadge(Rating rating) =>
        $"<span class=\"badge\" style=\"background:{ReportTheme.TierColor(rating.Tier)}\">{rating.Tier} {F(rating.Score, 0)}</span>";

    private static string Avatar(CharacterAnalysis c, string color)
    {
        if (string.IsNullOrEmpty(c.IconUrl))
        {
            return $"<span class=\"dot\" style=\"display:inline-block;background:{color}\"></span>";
        }

        return $"<img class=\"avatar\" src=\"{Enc(c.IconUrl)}\" alt=\"\" loading=\"lazy\" onerror=\"this.outerHTML='&lt;span class=&quot;dot&quot; style=&quot;display:inline-block;background:{color}&quot;&gt;&lt;/span&gt;'\">";
    }

    private static string Name(CharacterAnalysis c) => Bi(c.Name, string.IsNullOrEmpty(c.NameRu) ? c.Name : c.NameRu!);

    private static string MemberNames(TeamAnalysis t) => Bi(
        string.Join(", ", t.Members.Select(m => m.Name)),
        string.Join(", ", t.Members.Select(m => string.IsNullOrEmpty(m.NameRu) ? m.Name : m.NameRu!)));

    private static string RoleLabel(string role) => role switch
    {
        "Carry" => Bi("Carry", "Кэрри"),
        "Sub-DPS" => Bi("Sub-DPS", "Саб-ДПС"),
        _ => Bi("Support", "Саппорт"),
    };

    private static string PriorityLabel(RecommendationPriority p) => p switch
    {
        RecommendationPriority.High => Bi("HIGH", "СРОЧНО"),
        RecommendationPriority.Medium => Bi("MEDIUM", "СРЕДНЕ"),
        _ => Bi("LOW", "НИЗКО"),
    };

    private static string TopCharacterName(AccountAnalysis a) =>
        a.Characters.Count == 0 ? "—" : Name(a.Characters.MaxBy(c => c.OverallScore)!);

    private static string Bi(string en, string ru) =>
        $"<span class=\"en\">{Enc(en)}</span><span class=\"ru\">{Enc(ru)}</span>";

    private static string BiT(LocalizedText text) => Bi(text.En, text.Ru);

    private static string Enc(string? text) => WebUtility.HtmlEncode(text ?? string.Empty);

    private static string F(double value, int digits) =>
        value.ToString("F" + digits.ToString(Inv), Inv);
}
