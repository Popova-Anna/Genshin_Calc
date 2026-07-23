# Genshin Account Analyzer

A .NET 8 application for importing and analyzing Genshin Impact accounts (Enka.Network exports today;
HoYoLab / Akasha planned). Built on Clean Architecture with a source-agnostic import pipeline.

> Status: **Iteration 1** — solution scaffolding, source-independent JSON import, internal models,
> unit tests and the `POST /api/account/import` endpoint. Later iterations add analyzers, the damage
> calculator, the optimizer, HTML reports and an Angular UI (see the roadmap below).

## Architecture

Clean Architecture. Dependencies point inward — `Domain` depends on nothing; `Application` depends only
on `Domain`; adapters (`Parser`, `Infrastructure`) implement `Application` ports.

```
src/
  GenshinAccountAnalyzer.Domain          internal models + enums (no dependencies)
  GenshinAccountAnalyzer.Application      ports (IAccountImporter, IGameMetadataProvider), CQRS (MediatR), validation
  GenshinAccountAnalyzer.Infrastructure   port implementations (persistence/clients) — thin for now
  GenshinAccountAnalyzer.Parser           Enka DTOs + EnkaImporter (adapter for IAccountImporter)
  GenshinAccountAnalyzer.Calculator       damage formulas (later)
  GenshinAccountAnalyzer.Analyzer         character/artifact/weapon/team analysis (later)
  GenshinAccountAnalyzer.Report           HTML report generation (later)
  GenshinAccountAnalyzer.Api              ASP.NET Core Web API
tests/
  GenshinAccountAnalyzer.Parser.Tests     xUnit + FluentAssertions
```

### Source-independent import

Every data source implements one port:

```csharp
public interface IAccountImporter
{
    ImportSource Source { get; }
    Task<Account> ImportAsync(Stream stream, CancellationToken cancellationToken);
}
```

Adding `HoYoLabImporter` / `AkashaImporter` is a new class + one DI registration — no changes elsewhere.
Raw source DTOs never leave the `Parser` project; business logic only ever sees the internal `Account`
model. Static game metadata (names, elements, set names) is resolved through `IGameMetadataProvider`, so
supporting new characters/weapons is a **data** change, not a code change.

### Game metadata

Character names/elements/weapon types/rarities are resolved from an embedded data file
(`src/GenshinAccountAnalyzer.Parser/Resources/characters.json`) generated from Enka.Network's public
store. Regenerate it after a game update — no code changes required:

```bash
python tools/generate-metadata.py            # downloads the latest Enka store
python tools/generate-metadata.py --lang ru  # or a specific language
```

Unknown ids (e.g. characters newer than the store snapshot) fall back to safe placeholders. Weapon and
artifact-set names are not part of Enka's metadata and currently use placeholders (`Weapon <id>`,
`Set <id>`); they will be populated from a dedicated data source in a later iteration.

## Requirements

- .NET 8 SDK (the repo builds with newer SDKs via `global.json` roll-forward)

## Build & test

```bash
dotnet build GenshinAccountAnalyzer.sln
dotnet test GenshinAccountAnalyzer.sln
```

## Run the API

```bash
dotnet run --project src/GenshinAccountAnalyzer.Api
```

Then import an Enka export (Swagger UI is available in Development):

```bash
curl -X POST "http://localhost:5028/api/account/import?source=Enka" \
  -H "Content-Type: application/json" \
  --data-binary @GenshinData_<uid>.json
```

The response is the internal `Account` model (enums serialized as names).

Or import **and analyze** in one call (ratings, crit/ER/EM balance, efficiency per character):

```bash
curl -X POST "http://localhost:5028/api/account/analyze?source=Enka" \
  -H "Content-Type: application/json" \
  --data-binary @GenshinData_<uid>.json
```

An Enka export can be produced with the included `Get-GenshinData.ps1 -Uid <uid>` script.

## Roadmap

| Iteration | Scope |
|-----------|-------|
| 1 (done)  | Scaffolding, import pipeline, internal models, tests, import endpoint |
| 2 (done)  | Character Analyzer (levels, constellations, talents, crit/ER/EM balance, ratings, efficiency) |
| 3 (done)  | Artifact Analyzer (CV, roll value, efficiency, dead rolls, per-substat usefulness) |
| 4         | Weapon Analyzer (BiS, DPS loss) |
| 5         | Character Analyzer (strengths/weaknesses, upgrade priority) |
| 6         | Team Analyzer (resonance, reactions, energy) |
| 7         | Damage Calculator (full formulas, reactions, snapshot) |
| 8         | Optimizer |
| 9         | HTML Report |
| 10        | Angular 20 SPA |
