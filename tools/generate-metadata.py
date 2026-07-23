#!/usr/bin/env python3
"""Generate the parser's embedded character metadata from Enka.Network's public store.

This is a maintenance tool, not part of the build. Re-run it after a game update to refresh
`src/GenshinAccountAnalyzer.Parser/Resources/characters.json`; no code changes are needed to
support new characters.

Sources (Enka.Network API-docs store, CC-licensed community data):
  - characters.json : per-avatar Element / WeaponType / QualityType / NameTextMapHash
  - loc.json        : localized text keyed by language then text-map hash

Usage:
  python tools/generate-metadata.py [--lang en] [--source <dir-with-downloaded-files>]

When --source is omitted the files are downloaded from GitHub.
"""
from __future__ import annotations

import argparse
import io
import json
import os
import urllib.request

STORE_BASE = "https://raw.githubusercontent.com/EnkaNetwork/API-docs/master/store"
CHARACTERS_URL = f"{STORE_BASE}/characters.json"
LOC_URL = f"{STORE_BASE}/loc.json"

# Enka element name -> analyzer ElementType name.
ELEMENT_MAP = {
    "Wind": "Anemo",
    "Rock": "Geo",
    "Electric": "Electro",
    "Grass": "Dendro",
    "Water": "Hydro",
    "Fire": "Pyro",
    "Ice": "Cryo",
}

# Enka weapon name -> analyzer WeaponType name.
WEAPON_MAP = {
    "WEAPON_SWORD_ONE_HAND": "Sword",
    "WEAPON_CLAYMORE": "Claymore",
    "WEAPON_POLE": "Polearm",
    "WEAPON_BOW": "Bow",
    "WEAPON_CATALYST": "Catalyst",
}

# Enka quality -> star rarity.
QUALITY_MAP = {
    "QUALITY_ORANGE": 5,
    "QUALITY_ORANGE_SP": 5,
    "QUALITY_PURPLE": 4,
}

OUTPUT_PATH = os.path.join(
    os.path.dirname(__file__), "..",
    "src", "GenshinAccountAnalyzer.Parser", "Resources", "characters.json",
)


def _load(url: str, source_dir: str | None, filename: str) -> dict:
    if source_dir:
        path = os.path.join(source_dir, filename)
        with io.open(path, encoding="utf-8") as handle:
            return json.load(handle)
    with urllib.request.urlopen(url) as response:  # noqa: S310 (trusted static URL)
        return json.loads(response.read().decode("utf-8"))


def generate(lang: str, source_dir: str | None) -> dict[str, dict]:
    characters = _load(CHARACTERS_URL, source_dir, "characters.json")
    loc = _load(LOC_URL, source_dir, "loc.json").get(lang, {})

    result: dict[str, dict] = {}
    skipped: list[str] = []
    for avatar_id, data in characters.items():
        # Traveler entries are keyed "avatarId-skillDepotId" and have a per-depot element; skip for now.
        if "-" in avatar_id:
            continue

        element = ELEMENT_MAP.get(data.get("Element", ""))
        weapon = WEAPON_MAP.get(data.get("WeaponType", ""))
        rarity = QUALITY_MAP.get(data.get("QualityType", ""))
        name = loc.get(str(data.get("NameTextMapHash", "")))

        if not (element and weapon and rarity and name):
            skipped.append(avatar_id)
            continue

        # SkillOrder lists the three main upgradable talents (Normal Attack, Elemental Skill,
        # Elemental Burst) as skill ids, matching the keys used in an export's skillLevelMap.
        talents = [tid for tid in data.get("SkillOrder", []) if isinstance(tid, int)]

        result[avatar_id] = {
            "name": name,
            "element": element,
            "weapon": weapon,
            "rarity": rarity,
            "talents": talents,
        }

    if skipped:
        print(f"Skipped {len(skipped)} entries with incomplete data: {', '.join(skipped)}")
    return dict(sorted(result.items(), key=lambda kv: int(kv[0])))


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--lang", default="en", help="Localization language (default: en)")
    parser.add_argument("--source", default=None, help="Directory with pre-downloaded store files")
    args = parser.parse_args()

    metadata = generate(args.lang, args.source)

    output_path = os.path.abspath(OUTPUT_PATH)
    with io.open(output_path, "w", encoding="utf-8", newline="\n") as handle:
        json.dump(metadata, handle, ensure_ascii=False, indent=2)
        handle.write("\n")

    print(f"Wrote {len(metadata)} characters ({args.lang}) to {output_path}")


if __name__ == "__main__":
    main()
