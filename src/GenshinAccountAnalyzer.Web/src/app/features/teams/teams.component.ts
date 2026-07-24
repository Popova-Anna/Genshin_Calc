import { DecimalPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { LocalizedText } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';
import { elementColor } from '../../core/theme-colors';

/** The account's best auto-generated teams: reaction core, resonances, members and roles. */
@Component({
  selector: 'app-teams',
  imports: [DecimalPipe],
  templateUrl: './teams.component.html',
  styleUrl: './teams.component.scss',
})
export class TeamsComponent {
  private readonly store = inject(AccountStoreService);
  protected readonly language = inject(LanguageService);
  protected readonly elementColor = elementColor;

  readonly teams = computed(() => this.store.analysis()?.teams ?? []);

  protected pick(text: LocalizedText): string {
    return this.language.pick(text);
  }

  protected reasonsText(reasons: LocalizedText[]): string {
    return reasons.map((r) => this.pick(r)).join(' · ');
  }

  protected roleLabel(role: string): string {
    if (this.language.language() !== 'ru') {
      return role;
    }
    switch (role) {
      case 'Carry':
        return 'Кэрри';
      case 'Sub-DPS':
        return 'Саб-ДПС';
      default:
        return 'Саппорт';
    }
  }
}
