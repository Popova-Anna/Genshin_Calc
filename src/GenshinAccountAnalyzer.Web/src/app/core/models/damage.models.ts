// TypeScript mirrors of GenshinAccountAnalyzer.Domain.Damage records, as serialized by the API
// (camelCase, enums as strings). Keep in sync with the backend.

import {
  AdditiveReaction,
  AmplifyingReaction,
  ElementType,
  ScalingType,
  TransformativeReaction,
} from './enums';

export interface EnemyProfile {
  level: number;
  resistance: number;
  resistanceReduction: number;
  defenseReduction: number;
  defenseIgnore: number;
}

export interface DamageInput {
  characterLevel: number;
  talentMultiplier: number;
  scaling: ScalingType;
  scalingStatValue: number;
  flatDamageBonus: number;
  damageBonus: number;
  critRate: number;
  critDamage: number;
  elementalMastery: number;
  amplifying: AmplifyingReaction;
  triggerElement: ElementType;
  additive: AdditiveReaction;
  reactionBonus: number;
  enemy: EnemyProfile;
}

export interface DamageResult {
  nonCritical: number;
  critical: number;
  average: number;
}

export interface TransformativeHit {
  reaction: TransformativeReaction;
  characterLevel: number;
  elementalMastery: number;
  reactionBonus: number;
  enemy: EnemyProfile;
}

export interface RotationStep {
  name: string;
  hit?: DamageInput | null;
  transformative?: TransformativeHit | null;
}

export interface Rotation {
  name: string;
  durationSeconds: number;
  steps: RotationStep[];
}

export interface RotationStepResult {
  name: string;
  damage: DamageResult;
}

export interface RotationResult {
  steps: RotationStepResult[];
  totalNonCritical: number;
  totalCritical: number;
  totalAverage: number;
  dps: number;
}
