// Human-readable (bilingual) labels for the damage calculator's reaction pickers.
import { AdditiveReaction, AmplifyingReaction, ElementType, TransformativeReaction } from './models/enums';

/**
 * A single option for the unified "hit reaction" picker. In-game, a hit can trigger at most one of
 * an amplifying or an additive reaction (their auras are mutually exclusive) — this union keeps the
 * UI from ever producing an illegal combination the backend formulas were never meant to see together.
 */
export interface HitReactionOption {
  key: string;
  labelEn: string;
  labelRu: string;
  amplifying: AmplifyingReaction;
  triggerElement: ElementType;
  additive: AdditiveReaction;
}

export const HIT_REACTION_OPTIONS: readonly HitReactionOption[] = [
  { key: 'none', labelEn: 'None', labelRu: 'Нет', amplifying: 'None', triggerElement: 'Unknown', additive: 'None' },
  {
    key: 'vaporize-pyro',
    labelEn: 'Vaporize — Pyro hit (1.5×)',
    labelRu: 'Испарение — Пиро-хит (1.5×)',
    amplifying: 'Vaporize',
    triggerElement: 'Pyro',
    additive: 'None',
  },
  {
    key: 'vaporize-hydro',
    labelEn: 'Vaporize — Hydro hit (2×)',
    labelRu: 'Испарение — Гидро-хит (2×)',
    amplifying: 'Vaporize',
    triggerElement: 'Hydro',
    additive: 'None',
  },
  {
    key: 'melt-pyro',
    labelEn: 'Melt — Pyro hit (2×)',
    labelRu: 'Плавление — Пиро-хит (2×)',
    amplifying: 'Melt',
    triggerElement: 'Pyro',
    additive: 'None',
  },
  {
    key: 'melt-cryo',
    labelEn: 'Melt — Cryo hit (1.5×)',
    labelRu: 'Плавление — Крио-хит (1.5×)',
    amplifying: 'Melt',
    triggerElement: 'Cryo',
    additive: 'None',
  },
  {
    key: 'aggravate',
    labelEn: 'Aggravate',
    labelRu: 'Обострение',
    amplifying: 'None',
    triggerElement: 'Unknown',
    additive: 'Aggravate',
  },
  {
    key: 'spread',
    labelEn: 'Spread',
    labelRu: 'Растекание',
    amplifying: 'None',
    triggerElement: 'Unknown',
    additive: 'Spread',
  },
];

export const TRANSFORMATIVE_REACTION_LABELS: Record<TransformativeReaction, { en: string; ru: string }> = {
  Overloaded: { en: 'Overloaded', ru: 'Перегрузка' },
  Superconduct: { en: 'Superconduct', ru: 'Сверхпроводник' },
  ElectroCharged: { en: 'Electro-Charged', ru: 'Заряжен' },
  Swirl: { en: 'Swirl', ru: 'Рассеивание' },
  Shatter: { en: 'Shatter', ru: 'Дробление' },
  Burning: { en: 'Burning', ru: 'Горение' },
  Bloom: { en: 'Bloom (incl. Lunar-Bloom)', ru: 'Цветение (вкл. Лунное)' },
  Hyperbloom: { en: 'Hyperbloom (incl. Lunar)', ru: 'Гипербутонизация (вкл. Лунную)' },
  Burgeon: { en: 'Burgeon (incl. Lunar)', ru: 'Бутонизация (вкл. Лунную)' },
  Crystallize: { en: 'Crystallize (shield, no damage)', ru: 'Кристаллизация (щит, без урона)' },
  LunarCharged: { en: 'Lunar-Charged', ru: 'Лунный заряд' },
  LunarCrystallize: { en: 'Lunar-Crystallize', ru: 'Лунная кристаллизация' },
};
