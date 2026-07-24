import { Component, computed, inject, signal } from '@angular/core';
import { CharacterAnalysis, Recommendation } from '../../core/models/analysis.models';
import { RecommendationPriority } from '../../core/models/enums';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';
import { priorityColor } from '../../core/theme-colors';

interface RecommendationRow {
  characterName: string;
  rec: Recommendation;
}

const PRIORITY_ORDER: Record<RecommendationPriority, number> = { High: 2, Medium: 1, Low: 0 };

/** All per-character recommendations, sorted by priority and filterable. */
@Component({
  selector: 'app-recommendations',
  templateUrl: './recommendations.component.html',
  styleUrl: './recommendations.component.scss',
})
export class RecommendationsComponent {
  private readonly store = inject(AccountStoreService);
  protected readonly language = inject(LanguageService);
  protected readonly priorityColor = priorityColor;

  readonly priorityFilter = signal<RecommendationPriority | 'All'>('All');

  readonly rows = computed<RecommendationRow[]>(() => {
    const filter = this.priorityFilter();
    const characters: CharacterAnalysis[] = this.store.characters();

    return characters
      .flatMap((c) =>
        c.recommendations
          .filter((rec) => filter === 'All' || rec.priority === filter)
          .map((rec) => ({
            characterName: this.language.pickName(c.name, c.nameRu),
            rec,
          })),
      )
      .sort((a, b) => PRIORITY_ORDER[b.rec.priority] - PRIORITY_ORDER[a.rec.priority]);
  });
}
