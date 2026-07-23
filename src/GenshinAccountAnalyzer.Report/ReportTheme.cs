using GenshinAccountAnalyzer.Domain.Analysis;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Report;

/// <summary>Colour tokens and the stylesheet for the HTML report.</summary>
internal static class ReportTheme
{
    /// <summary>Returns the accent colour for an element.</summary>
    /// <param name="element">The element.</param>
    public static string ElementColor(ElementType element) => element switch
    {
        ElementType.Anemo => "#74c2a8",
        ElementType.Geo => "#f5b94c",
        ElementType.Electro => "#b18fdb",
        ElementType.Dendro => "#a5c83b",
        ElementType.Hydro => "#4cb9f5",
        ElementType.Pyro => "#ef7a52",
        ElementType.Cryo => "#7fd3e8",
        ElementType.Physical => "#cfd3e0",
        _ => "#8a93b0",
    };

    /// <summary>Returns the colour for a rating tier.</summary>
    /// <param name="tier">The tier.</param>
    public static string TierColor(RatingTier tier) => tier switch
    {
        RatingTier.SS => "#ffd66b",
        RatingTier.S => "#ff8a5b",
        RatingTier.A => "#b16bff",
        RatingTier.B => "#5b9dff",
        RatingTier.C => "#4fd1c5",
        RatingTier.D => "#8a93b0",
        _ => "#6b7280",
    };

    /// <summary>Returns the colour for a recommendation priority.</summary>
    /// <param name="priority">The priority.</param>
    public static string PriorityColor(RecommendationPriority priority) => priority switch
    {
        RecommendationPriority.High => "#ef5b6b",
        RecommendationPriority.Medium => "#f5b94c",
        _ => "#5b9dff",
    };

    /// <summary>The report stylesheet, inlined for a self-contained document.</summary>
    public const string Css = """
        :root { --bg:#0d1020; --bg2:#141834; --card:#1a1f3d; --line:#2a3157; --text:#e8e9f5; --muted:#9aa0c8; --accent:#7c8cff; }
        * { box-sizing:border-box; }
        body { margin:0; font-family:'Segoe UI',system-ui,-apple-system,Roboto,sans-serif; background:var(--bg); color:var(--text); line-height:1.5; }
        a { color:inherit; text-decoration:none; }
        .wrap { max-width:1180px; margin:0 auto; padding:0 20px; }
        header.hero { background:radial-gradient(1200px 400px at 20% -10%, #23306b 0%, transparent 60%), var(--bg2); border-bottom:1px solid var(--line); padding:36px 0 28px; }
        .hero-grid { display:flex; align-items:center; gap:28px; flex-wrap:wrap; }
        .gauge { --v:0; width:120px; height:120px; border-radius:50%; display:grid; place-items:center; background:conic-gradient(var(--accent) calc(var(--v)*1%), #2a3157 0); flex:0 0 auto; }
        .gauge > div { width:92px; height:92px; border-radius:50%; background:var(--bg2); display:grid; place-items:center; text-align:center; }
        .gauge b { font-size:26px; } .gauge span { font-size:11px; color:var(--muted); }
        h1 { margin:0 0 4px; font-size:26px; } .sub { color:var(--muted); font-size:14px; }
        .tiles { display:flex; gap:14px; flex-wrap:wrap; margin-top:6px; }
        .tile { background:var(--card); border:1px solid var(--line); border-radius:12px; padding:10px 16px; min-width:120px; }
        .tile b { font-size:20px; display:block; } .tile span { font-size:12px; color:var(--muted); }
        nav { position:sticky; top:0; z-index:5; background:rgba(13,16,32,.92); backdrop-filter:blur(6px); border-bottom:1px solid var(--line); }
        nav .wrap { display:flex; gap:6px; flex-wrap:wrap; padding-top:10px; padding-bottom:10px; }
        nav a { font-size:13px; color:var(--muted); padding:5px 11px; border-radius:8px; } nav a:hover { color:var(--text); background:var(--card); }
        section { padding:30px 0 8px; } section > .wrap > h2 { font-size:19px; margin:0 0 16px; display:flex; align-items:center; gap:10px; }
        section > .wrap > h2::before { content:""; width:4px; height:18px; border-radius:2px; background:var(--accent); }
        .grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(300px,1fr)); gap:16px; }
        .card { background:var(--card); border:1px solid var(--line); border-radius:14px; padding:16px; }
        .card-h { display:flex; align-items:center; justify-content:space-between; gap:10px; margin-bottom:12px; }
        .name { font-weight:600; } .meta { color:var(--muted); font-size:12px; }
        .chip { display:inline-block; font-size:11px; padding:2px 9px; border-radius:20px; border:1px solid var(--line); }
        .badge { font-weight:700; font-size:13px; padding:3px 10px; border-radius:8px; color:#12152a; }
        .bar { height:8px; border-radius:6px; background:#2a3157; overflow:hidden; margin:3px 0 9px; }
        .bar > i { display:block; height:100%; border-radius:6px; }
        .rowlbl { display:flex; justify-content:space-between; font-size:12px; color:var(--muted); }
        .statrow { display:flex; gap:8px; flex-wrap:wrap; margin-top:6px; }
        .kv { font-size:12px; background:#12152e; border:1px solid var(--line); border-radius:8px; padding:4px 8px; }
        .kv b { color:var(--text); }
        .tags { display:flex; gap:6px; flex-wrap:wrap; margin-top:10px; }
        .good { color:#7fe0a5; border-color:#2f5a44; } .bad { color:#ffb0a0; border-color:#5a3630; }
        table { width:100%; border-collapse:collapse; font-size:13px; }
        th,td { text-align:left; padding:9px 10px; border-bottom:1px solid var(--line); }
        th { color:var(--muted); font-weight:500; font-size:12px; }
        .distrow { display:flex; align-items:center; gap:10px; margin:6px 0; font-size:13px; }
        .distrow .lbl { width:90px; color:var(--muted); } .distrow .bar { flex:1; margin:0; }
        .rec { display:flex; gap:10px; align-items:flex-start; padding:10px 0; border-bottom:1px solid var(--line); }
        .prio { font-size:11px; font-weight:700; padding:2px 8px; border-radius:6px; color:#12152a; flex:0 0 auto; }
        .members { display:flex; flex-direction:column; gap:6px; margin:10px 0; }
        .member { display:flex; align-items:center; gap:8px; font-size:13px; } .role { margin-left:auto; color:var(--muted); font-size:12px; }
        .dot { width:10px; height:10px; border-radius:50%; flex:0 0 auto; }
        footer { color:var(--muted); font-size:12px; text-align:center; padding:26px 0 40px; }
        @media (max-width:520px){ .gauge{width:96px;height:96px} .hero-grid{gap:18px} }
        """;
}
