import { DecimalPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ImportPanelComponent } from '../import-panel/import-panel.component';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';
import { tierColor } from '../../core/theme-colors';

/** Landing page: import prompt when no account is loaded, overview dashboard once one is. */
@Component({
  selector: 'app-home',
  imports: [ImportPanelComponent, RouterLink, DecimalPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  protected readonly store = inject(AccountStoreService);
  protected readonly language = inject(LanguageService);
  protected readonly tierColor = tierColor;

  protected readonly strongCount = computed(
    () => this.store.characters().filter((c) => c.buildRating.score >= 85).length,
  );

  protected readonly topCharacter = computed(() => {
    const chars = this.store.characters();
    return chars.length === 0
      ? null
      : chars.reduce((best, c) => (c.overallScore > best.overallScore ? c : best));
  });

  protected readonly topTeam = computed(() => this.store.analysis()?.teams[0] ?? null);

  protected teamMemberNames(team: NonNullable<ReturnType<HomeComponent['topTeam']>>): string {
    return team.members.map((m) => this.language.pickName(m.name, m.nameRu)).join(', ');
  }
}
