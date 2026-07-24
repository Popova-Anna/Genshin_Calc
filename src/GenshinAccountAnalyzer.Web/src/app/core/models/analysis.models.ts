// TypeScript mirrors of GenshinAccountAnalyzer.Domain.Analysis records, as serialized by the API
// (camelCase, enums as strings). Keep in sync with the backend.

import { ArtifactSlot, ElementType, RatingTier, RecommendationPriority, StatType, WeaponType } from './enums';

export interface LocalizedText {
  en: string;
  ru: string;
}

export interface Rating {
  score: number;
  tier: RatingTier;
}

export interface TalentLevels {
  normalAttack: number;
  elementalSkill: number;
  elementalBurst: number;
}

export interface CritBalance {
  critRate: number;
  critDamage: number;
  critValue: number;
  ratio: number;
  balanceScore: number;
  isBalanced: boolean;
}

export interface SubstatAnalysis {
  type: StatType;
  value: number;
  rollValue: number;
  weight: number;
  usefulness: number;
  isDead: boolean;
}

export interface ArtifactAnalysis {
  artifactId: number;
  slot: ArtifactSlot;
  setName: string;
  level: number;
  rarity: number;
  critValue: number;
  rollValue: number;
  rollCount: number;
  efficiency: number;
  deadRolls: number;
  substats: SubstatAnalysis[];
  profileName: string;
}

export interface WeaponOption {
  id: number;
  name: string;
  rarity: number;
  score: number;
  relativeToBis: number;
}

export interface WeaponAnalysis {
  characterId: number;
  weaponType: WeaponType;
  equipped: WeaponOption | null;
  recommendations: WeaponOption[];
  dpsLossVsBis: number;
  profileName: string;
}

export interface EquippedSet {
  setId: number;
  setName: string;
  pieceCount: number;
}

export interface ArtifactRecommendation {
  mainStats: Partial<Record<ArtifactSlot, StatType>>;
  substats: StatType[];
  currentSets: EquippedSet[];
  notes: string;
}

export interface Recommendation {
  category: string;
  title: LocalizedText;
  detail: LocalizedText;
  priority: RecommendationPriority;
}

export interface SlotOptimization {
  slot: ArtifactSlot;
  currentMain: StatType;
  recommendedMain: StatType;
  isOptimal: boolean;
}

export interface BuildOptimization {
  characterId: number;
  slots: SlotOptimization[];
  bestWeapon: WeaponOption | null;
  optimizationScore: number;
  notes: LocalizedText[];
}

export interface TeamMember {
  characterId: number;
  name: string;
  nameRu: string | null;
  element: ElementType;
  buildScore: number;
  role: string;
}

export interface TeamResonance {
  element: ElementType;
  name: LocalizedText;
  effect: LocalizedText;
}

export interface TeamAnalysis {
  members: TeamMember[];
  score: number;
  reactionCore: LocalizedText;
  resonances: TeamResonance[];
  reasons: LocalizedText[];
}

export interface CharacterAnalysis {
  characterId: number;
  name: string;
  nameRu: string | null;
  iconUrl: string | null;
  element: ElementType;
  level: number;
  maxLevel: number;
  constellationLevel: number;
  talents: TalentLevels | null;
  talentRating: Rating;
  weaponRating: Rating;
  artifactRating: Rating;
  buildRating: Rating;
  overallScore: number;
  critBalance: CritBalance;
  energyRecharge: number;
  elementalMastery: number;
  efficiency: number;
  artifacts: ArtifactAnalysis[];
  weapon: WeaponAnalysis | null;
  strengths: LocalizedText[];
  weaknesses: LocalizedText[];
  recommendations: Recommendation[];
  bestWeapon: WeaponOption | null;
  bestArtifacts: ArtifactRecommendation | null;
  bestTeams: TeamAnalysis[];
  optimization: BuildOptimization | null;
}

export interface AccountAnalysis {
  uid: string;
  characters: CharacterAnalysis[];
  averageBuildScore: number;
  teams: TeamAnalysis[];
}
