import { Injectable, signal } from '@angular/core';
import { LocalizedText } from '../models/analysis.models';
import { RecommendationPriority } from '../models/enums';

export type Language = 'en' | 'ru';

const PRIORITY_LABELS_RU: Record<RecommendationPriority, string> = {
  High: 'Срочно',
  Medium: 'Средне',
  Low: 'Низко',
};

const STORAGE_KEY = 'gaa-lang';

/** Manages the display language (English/Russian), persisted in localStorage. */
@Injectable({ providedIn: 'root' })
export class LanguageService {
  readonly language = signal<Language>(this.readInitialLanguage());

  toggle(): void {
    this.set(this.language() === 'en' ? 'ru' : 'en');
  }

  set(language: Language): void {
    this.language.set(language);
    localStorage.setItem(STORAGE_KEY, language);
  }

  /** Picks the active-language string from a bilingual text pair. */
  pick(text: LocalizedText): string {
    return this.language() === 'ru' ? text.ru : text.en;
  }

  /** Picks a character's Russian name when available and Russian is active, else the English name. */
  pickName(name: string, nameRu: string | null | undefined): string {
    return this.language() === 'ru' && nameRu ? nameRu : name;
  }

  /** Localizes a recommendation priority badge label. */
  priorityLabel(priority: RecommendationPriority): string {
    return this.language() === 'ru' ? PRIORITY_LABELS_RU[priority] : priority;
  }

  private readInitialLanguage(): Language {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'ru' || stored === 'en' ? stored : 'en';
  }
}
