// Mirrors GenshinAccountAnalyzer.Report/ReportTheme.cs so the SPA and the static HTML report agree
// visually on element/tier/priority colours.
import { ElementType, RatingTier, RecommendationPriority } from './models/enums';

const ELEMENT_COLORS: Record<ElementType, string> = {
  Anemo: '#74c2a8',
  Geo: '#f5b94c',
  Electro: '#b18fdb',
  Dendro: '#a5c83b',
  Hydro: '#4cb9f5',
  Pyro: '#ef7a52',
  Cryo: '#7fd3e8',
  Physical: '#cfd3e0',
  Unknown: '#8a93b0',
};

const TIER_COLORS: Record<RatingTier, string> = {
  SS: '#ffd66b',
  S: '#ff8a5b',
  A: '#b16bff',
  B: '#5b9dff',
  C: '#4fd1c5',
  D: '#8a93b0',
  F: '#6b7280',
};

const PRIORITY_COLORS: Record<RecommendationPriority, string> = {
  High: '#ef5b6b',
  Medium: '#f5b94c',
  Low: '#5b9dff',
};

export function elementColor(element: ElementType): string {
  return ELEMENT_COLORS[element] ?? ELEMENT_COLORS.Unknown;
}

export function tierColor(tier: RatingTier): string {
  return TIER_COLORS[tier] ?? TIER_COLORS.F;
}

export function priorityColor(priority: RecommendationPriority): string {
  return PRIORITY_COLORS[priority] ?? PRIORITY_COLORS.Low;
}
