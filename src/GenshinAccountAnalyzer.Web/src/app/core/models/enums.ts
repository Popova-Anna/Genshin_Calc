// Mirrors the enum string values produced by the API's JsonStringEnumConverter
// (GenshinAccountAnalyzer.Domain.Enums / .Analysis). Keep in sync with the backend enums.

export type ElementType =
  | 'Unknown'
  | 'Anemo'
  | 'Geo'
  | 'Electro'
  | 'Dendro'
  | 'Hydro'
  | 'Pyro'
  | 'Cryo'
  | 'Physical';

export type WeaponType = 'Unknown' | 'Sword' | 'Claymore' | 'Polearm' | 'Bow' | 'Catalyst';

export type ArtifactSlot = 'Unknown' | 'Flower' | 'Plume' | 'Sands' | 'Goblet' | 'Circlet';

export type StatType =
  | 'None'
  | 'BaseHp'
  | 'Hp'
  | 'HpPercent'
  | 'BaseAtk'
  | 'Atk'
  | 'AtkPercent'
  | 'BaseDef'
  | 'Def'
  | 'DefPercent'
  | 'ElementalMastery'
  | 'CritRate'
  | 'CritDamage'
  | 'EnergyRecharge'
  | 'HealingBonus'
  | 'IncomingHealingBonus'
  | 'PhysicalDamageBonus'
  | 'PyroDamageBonus'
  | 'HydroDamageBonus'
  | 'DendroDamageBonus'
  | 'ElectroDamageBonus'
  | 'AnemoDamageBonus'
  | 'CryoDamageBonus'
  | 'GeoDamageBonus';

export type RatingTier = 'F' | 'D' | 'C' | 'B' | 'A' | 'S' | 'SS';

export type RecommendationPriority = 'Low' | 'Medium' | 'High';

export type ImportSource = 'Unknown' | 'Enka' | 'HoYoLab' | 'Akasha';

// --- Domain.Damage enums (GenshinAccountAnalyzer.Calculator formulas) ---

export type ScalingType = 'Atk' | 'Hp' | 'Def';

export type AmplifyingReaction = 'None' | 'Vaporize' | 'Melt';

export type AdditiveReaction = 'None' | 'Aggravate' | 'Spread';

export type TransformativeReaction =
  | 'Overloaded'
  | 'Superconduct'
  | 'ElectroCharged'
  | 'Swirl'
  | 'Shatter'
  | 'Burning'
  | 'Bloom'
  | 'Hyperbloom'
  | 'Burgeon'
  | 'Crystallize'
  | 'LunarCharged'
  | 'LunarCrystallize';
