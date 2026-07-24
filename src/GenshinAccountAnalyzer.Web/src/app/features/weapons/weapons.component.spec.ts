import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountAnalysis } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { WeaponsComponent } from './weapons.component';

describe('WeaponsComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [WeaponsComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
  });

  it('builds one row per character sorted by overall score', () => {
    const store = TestBed.inject(AccountStoreService);
    (store as unknown as { _analysis: { set: (v: AccountAnalysis) => void } })['_analysis'].set({
      uid: '1',
      averageBuildScore: 0,
      teams: [],
      characters: [
        {
          characterId: 1,
          name: 'Zhongli',
          nameRu: null,
          overallScore: 70,
          weapon: { equipped: { name: 'Black Tassel' }, dpsLossVsBis: 57 },
          bestWeapon: { name: 'Staff of Homa' },
        },
        {
          characterId: 2,
          name: 'Kazuha',
          nameRu: null,
          overallScore: 92,
          weapon: { equipped: { name: 'Freedom-Sworn' }, dpsLossVsBis: 14 },
          bestWeapon: { name: 'Mistsplitter Reforged' },
        },
      ] as never,
    });

    const fixture = TestBed.createComponent(WeaponsComponent);
    fixture.detectChanges();

    const rows = fixture.componentInstance.rows();
    expect(rows.map((r) => r.characterId)).toEqual([2, 1]);
    expect(rows[0].equipped).toBe('Freedom-Sworn');
    expect(rows[0].dpsLoss).toBe(14);
    expect(rows[0].bestInSlot).toBe('Mistsplitter Reforged');
  });

  it('falls back to placeholders when weapon data is missing', () => {
    const store = TestBed.inject(AccountStoreService);
    (store as unknown as { _analysis: { set: (v: AccountAnalysis) => void } })['_analysis'].set({
      uid: '1',
      averageBuildScore: 0,
      teams: [],
      characters: [
        { characterId: 1, name: 'Test', nameRu: null, overallScore: 50, weapon: null, bestWeapon: null },
      ] as never,
    });

    const fixture = TestBed.createComponent(WeaponsComponent);
    fixture.detectChanges();

    const row = fixture.componentInstance.rows()[0];
    expect(row.equipped).toBe('—');
    expect(row.bestInSlot).toBe('—');
    expect(row.dpsLoss).toBe(0);
  });
});
