import { DecimalPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';

interface WeaponRow {
  characterId: number;
  name: string;
  equipped: string;
  dpsLoss: number;
  bestInSlot: string;
}

/** Per-character weapon table: equipped weapon, loss vs best-in-slot, and the suggested upgrade. */
@Component({
  selector: 'app-weapons',
  imports: [DecimalPipe],
  templateUrl: './weapons.component.html',
  styleUrl: './weapons.component.scss',
})
export class WeaponsComponent {
  private readonly store = inject(AccountStoreService);
  protected readonly language = inject(LanguageService);

  readonly rows = computed<WeaponRow[]>(() =>
    [...this.store.characters()]
      .sort((a, b) => b.overallScore - a.overallScore)
      .map((c) => ({
        characterId: c.characterId,
        name: this.language.pickName(c.name, c.nameRu),
        equipped: c.weapon?.equipped?.name ?? '—',
        dpsLoss: c.weapon?.dpsLossVsBis ?? 0,
        bestInSlot: c.bestWeapon?.name ?? '—',
      })),
  );
}
