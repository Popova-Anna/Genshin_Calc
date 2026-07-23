#!/usr/bin/env node
/*
 * Generate the parser's embedded weapon catalog from genshin-db.
 *
 * Maintenance tool, not part of the build. Re-run after a game update to refresh
 * src/GenshinAccountAnalyzer.Parser/Resources/weapons.json (no code changes needed to add weapons).
 *
 * Usage:
 *   cd tools && npm init -y && npm i genshin-db && node generate-weapons.js
 *
 * Output units follow the analyzer's domain convention: percentage secondary stats are stored as
 * fractions (e.g. 44.1% CRIT DMG -> 0.441); flat secondaries (EM, ATK) are stored as-is.
 */
'use strict';

const fs = require('fs');
const path = require('path');
const genshindb = require('genshin-db');

const MAX_LEVEL = 90;
const MIN_RARITY = 3; // 1★/2★ weapons cannot reach level 90 and are never build-relevant.

const WEAPON_TYPE = {
  WEAPON_SWORD_ONE_HAND: 'Sword',
  WEAPON_CLAYMORE: 'Claymore',
  WEAPON_POLE: 'Polearm',
  WEAPON_BOW: 'Bow',
  WEAPON_CATALYST: 'Catalyst',
};

const SECONDARY_STAT = {
  FIGHT_PROP_HP_PERCENT: 'HpPercent',
  FIGHT_PROP_ATTACK_PERCENT: 'AtkPercent',
  FIGHT_PROP_DEFENSE_PERCENT: 'DefPercent',
  FIGHT_PROP_CRITICAL: 'CritRate',
  FIGHT_PROP_CRITICAL_HURT: 'CritDamage',
  FIGHT_PROP_CHARGE_EFFICIENCY: 'EnergyRecharge',
  FIGHT_PROP_ELEMENT_MASTERY: 'ElementalMastery',
  FIGHT_PROP_PHYSICAL_ADD_HURT: 'PhysicalDamageBonus',
};

const outputPath = path.join(
  __dirname, '..',
  'src', 'GenshinAccountAnalyzer.Parser', 'Resources', 'weapons.json',
);

function round(value, digits) {
  const factor = 10 ** digits;
  return Math.round(value * factor) / factor;
}

function main() {
  const names = genshindb.weapons('names', { matchCategories: true });
  const result = {};
  const skipped = [];

  for (const name of names) {
    const weapon = genshindb.weapons(name);
    if (!weapon || weapon.rarity < MIN_RARITY) {
      continue;
    }

    const type = WEAPON_TYPE[weapon.weaponType];
    if (!type) {
      skipped.push(`${weapon.id} (type ${weapon.weaponType})`);
      continue;
    }

    let stats;
    try {
      stats = weapon.stats(MAX_LEVEL);
    } catch {
      skipped.push(`${weapon.id} (no lvl90 stats)`);
      continue;
    }

    const secondary = SECONDARY_STAT[weapon.mainStatType] ?? 'None';
    result[weapon.id] = {
      name: weapon.name,
      type,
      rarity: weapon.rarity,
      baseAttack: round(stats.attack, 2),
      secondaryStat: secondary,
      secondaryValue: secondary === 'None' ? 0 : round(stats.specialized ?? 0, 4),
    };
  }

  const sorted = Object.fromEntries(
    Object.entries(result).sort((a, b) => Number(a[0]) - Number(b[0])),
  );

  fs.writeFileSync(outputPath, JSON.stringify(sorted, null, 2) + '\n', 'utf-8');
  if (skipped.length) {
    console.log(`Skipped ${skipped.length}: ${skipped.join(', ')}`);
  }
  console.log(`Wrote ${Object.keys(sorted).length} weapons to ${outputPath}`);
}

main();
