import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, input, output } from '@angular/core';
import { CharacterAnalysis, LocalizedText, Rating } from '../../../core/models/analysis.models';
import { LanguageService } from '../../../core/services/language.service';
import { elementColor, priorityColor, tierColor } from '../../../core/theme-colors';

interface RatingRow {
  labelEn: string;
  labelRu: string;
  rating: Rating;
}

/** A single character's summary card, with an expandable detail panel. */
@Component({
  selector: 'app-character-card',
  imports: [DecimalPipe],
  templateUrl: './character-card.component.html',
  styleUrl: './character-card.component.scss',
})
export class CharacterCardComponent {
  protected readonly language = inject(LanguageService);
  protected readonly elementColor = elementColor;
  protected readonly tierColor = tierColor;
  protected readonly priorityColor = priorityColor;

  readonly character = input.required<CharacterAnalysis>();
  readonly expanded = input(false);
  readonly toggleExpand = output<void>();

  protected readonly displayName = computed(() =>
    this.language.pickName(this.character().name, this.character().nameRu),
  );

  protected readonly ratingRows = computed<RatingRow[]>(() => {
    const c = this.character();
    return [
      { labelEn: 'Talents', labelRu: 'Таланты', rating: c.talentRating },
      { labelEn: 'Weapon', labelRu: 'Оружие', rating: c.weaponRating },
      { labelEn: 'Artifacts', labelRu: 'Артефакты', rating: c.artifactRating },
    ];
  });

  protected pick(text: LocalizedText): string {
    return this.language.pick(text);
  }

  protected weaponSuggestionNames(weapon: CharacterAnalysis['weapon']): string {
    return (weapon?.recommendations ?? []).map((r) => r.name).join(', ');
  }

  protected teamMemberNames(team: CharacterAnalysis['bestTeams'][number]): string {
    return team.members.map((m) => this.language.pickName(m.name, m.nameRu)).join(', ');
  }
}
