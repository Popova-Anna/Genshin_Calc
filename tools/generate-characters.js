#!/usr/bin/env node
/*
 * Generate the parser's embedded character metadata from genshin-db (names in English + Russian,
 * element, weapon, rarity, icon) merged with Enka.Network's talent skill order.
 *
 * Maintenance tool, not part of the build. Re-run after a game update:
 *   cd tools && npm init -y && npm i genshin-db && node generate-characters.js
 *
 * genshin-db is complete and up to date (covers characters newer than the Enka store snapshot);
 * Enka supplies the three main talent skill ids (skillLevelMap keys) where available.
 */
'use strict';

const fs = require('fs');
const path = require('path');
const genshindb = require('genshin-db');

const ENKA_CHARACTERS = 'https://raw.githubusercontent.com/EnkaNetwork/API-docs/master/store/characters.json';

const ELEMENT = {
  ELEMENT_ANEMO: 'Anemo', ELEMENT_GEO: 'Geo', ELEMENT_ELECTRO: 'Electro',
  ELEMENT_DENDRO: 'Dendro', ELEMENT_HYDRO: 'Hydro', ELEMENT_PYRO: 'Pyro', ELEMENT_CRYO: 'Cryo',
};

const WEAPON = {
  WEAPON_SWORD_ONE_HAND: 'Sword', WEAPON_CLAYMORE: 'Claymore', WEAPON_POLE: 'Polearm',
  WEAPON_BOW: 'Bow', WEAPON_CATALYST: 'Catalyst',
};

const outputPath = path.join(
  __dirname, '..',
  'src', 'GenshinAccountAnalyzer.Parser', 'Resources', 'characters.json',
);

async function fetchEnkaTalents() {
  const map = {};
  try {
    const response = await fetch(ENKA_CHARACTERS);
    const data = await response.json();
    for (const [key, entry] of Object.entries(data)) {
      if (!key.includes('-') && Array.isArray(entry.SkillOrder)) {
        map[key] = entry.SkillOrder.filter((n) => Number.isInteger(n));
      }
    }
  } catch (error) {
    console.warn(`Could not fetch Enka talents (${error.message}); talents will be omitted.`);
  }
  return map;
}

function characterById() {
  const names = genshindb.characters('names', { matchCategories: true });
  const byId = {};
  for (const name of names) {
    const character = genshindb.characters(name);
    if (character && character.id) {
      byId[character.id] = character;
    }
  }
  return byId;
}

async function main() {
  const talents = await fetchEnkaTalents();
  const byId = characterById();
  const result = {};
  const skipped = [];

  for (const [id, character] of Object.entries(byId)) {
    const element = ELEMENT[character.elementType];
    const weapon = WEAPON[character.weaponType];
    if (!element || !weapon) {
      skipped.push(`${id} (${character.name})`);
      continue;
    }

    const russian = genshindb.characters(character.name, { resultLanguage: genshindb.Language.Russian });

    result[id] = {
      nameEn: character.name,
      nameRu: (russian && russian.name) || character.name,
      element,
      weapon,
      rarity: character.rarity,
      icon: character.images && character.images.filename_icon ? character.images.filename_icon : '',
      talents: talents[id] || [],
    };
  }

  const sorted = Object.fromEntries(Object.entries(result).sort((a, b) => Number(a[0]) - Number(b[0])));
  fs.writeFileSync(outputPath, JSON.stringify(sorted, null, 2) + '\n', 'utf-8');

  if (skipped.length) {
    console.log(`Skipped ${skipped.length}: ${skipped.join(', ')}`);
  }
  console.log(`Wrote ${Object.keys(sorted).length} characters to ${outputPath}`);
}

main();
