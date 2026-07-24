import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CharacterCardComponent } from './character-card/character-card.component';
import { CharacterAnalysis } from '../../core/models/analysis.models';
import { ElementType } from '../../core/models/enums';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';

type SortKey = 'score' | 'level' | 'name' | 'efficiency';

const ELEMENTS: ElementType[] = [
  'Anemo',
  'Geo',
  'Electro',
  'Dendro',
  'Hydro',
  'Pyro',
  'Cryo',
  'Physical',
];

/** Character roster: searchable, filterable by element, sortable, with expandable detail cards. */
@Component({
  selector: 'app-characters',
  imports: [CharacterCardComponent, FormsModule],
  templateUrl: './characters.component.html',
  styleUrl: './characters.component.scss',
})
export class CharactersComponent {
  private readonly store = inject(AccountStoreService);
  protected readonly language = inject(LanguageService);
  protected readonly elements = ELEMENTS;

  readonly search = signal('');
  readonly elementFilter = signal<ElementType | 'All'>('All');
  readonly sortKey = signal<SortKey>('score');
  private readonly expandedIds = signal<ReadonlySet<number>>(new Set());

  readonly filtered = computed<CharacterAnalysis[]>(() => {
    const term = this.search().trim().toLowerCase();
    const element = this.elementFilter();
    const sort = this.sortKey();

    let list = this.store.characters();

    if (term) {
      list = list.filter(
        (c) =>
          c.name.toLowerCase().includes(term) ||
          (c.nameRu?.toLowerCase().includes(term) ?? false),
      );
    }

    if (element !== 'All') {
      list = list.filter((c) => c.element === element);
    }

    return [...list].sort((a, b) => {
      switch (sort) {
        case 'level':
          return b.level - a.level;
        case 'name':
          return a.name.localeCompare(b.name);
        case 'efficiency':
          return b.efficiency - a.efficiency;
        case 'score':
        default:
          return b.overallScore - a.overallScore;
      }
    });
  });

  isExpanded(id: number): boolean {
    return this.expandedIds().has(id);
  }

  toggleExpand(id: number): void {
    const next = new Set(this.expandedIds());
    if (next.has(id)) {
      next.delete(id);
    } else {
      next.add(id);
    }
    this.expandedIds.set(next);
  }
}
