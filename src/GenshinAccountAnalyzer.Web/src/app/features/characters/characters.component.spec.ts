import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountAnalysis } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { CharactersComponent } from './characters.component';

function withCharacters(chars: unknown[]): AccountAnalysis {
  return { uid: '1', averageBuildScore: 0, teams: [], characters: chars as never };
}

describe('CharactersComponent', () => {
  let store: AccountStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [CharactersComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
    store = TestBed.inject(AccountStoreService);
    (store as unknown as { _analysis: { set: (v: AccountAnalysis) => void } })['_analysis'].set(
      withCharacters([
        {
          characterId: 1,
          name: 'Kazuha',
          nameRu: 'Кадзуха',
          element: 'Anemo',
          level: 90,
          overallScore: 92,
          efficiency: 100,
          buildRating: { score: 92, tier: 'S' },
          talentRating: { score: 100, tier: 'SS' },
          weaponRating: { score: 84, tier: 'A' },
          artifactRating: { score: 88, tier: 'A' },
          critBalance: { critRate: 38, critDamage: 184, critValue: 260, ratio: 4.9, balanceScore: 60, isBalanced: false },
          energyRecharge: 1.22,
          elementalMastery: 577,
          strengths: [],
          weaknesses: [],
          recommendations: [],
          bestTeams: [],
        },
        {
          characterId: 2,
          name: 'Zhongli',
          nameRu: 'Чжун Ли',
          element: 'Geo',
          level: 80,
          overallScore: 70,
          efficiency: 90,
          buildRating: { score: 70, tier: 'B' },
          talentRating: { score: 80, tier: 'A' },
          weaponRating: { score: 60, tier: 'B' },
          artifactRating: { score: 70, tier: 'B' },
          critBalance: { critRate: 26, critDamage: 123, critValue: 175, ratio: 4.7, balanceScore: 40, isBalanced: false },
          energyRecharge: 1.09,
          elementalMastery: 77,
          strengths: [],
          weaknesses: [],
          recommendations: [],
          bestTeams: [],
        },
      ]),
    );
  });

  it('lists all characters sorted by build score by default', () => {
    const fixture = TestBed.createComponent(CharactersComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.filtered().map((c) => c.characterId)).toEqual([1, 2]);
  });

  it('filters by search term (English or Russian name)', () => {
    const fixture = TestBed.createComponent(CharactersComponent);
    const component = fixture.componentInstance;

    component.search.set('чжун');
    fixture.detectChanges();

    expect(component.filtered().map((c) => c.characterId)).toEqual([2]);
  });

  it('filters by element', () => {
    const fixture = TestBed.createComponent(CharactersComponent);
    const component = fixture.componentInstance;

    component.elementFilter.set('Geo');
    fixture.detectChanges();

    expect(component.filtered().map((c) => c.characterId)).toEqual([2]);
  });

  it('sorts by level when requested', () => {
    const fixture = TestBed.createComponent(CharactersComponent);
    const component = fixture.componentInstance;

    component.sortKey.set('level');
    fixture.detectChanges();

    expect(component.filtered().map((c) => c.characterId)).toEqual([1, 2]);
  });

  it('toggles card expansion independently per character', () => {
    const fixture = TestBed.createComponent(CharactersComponent);
    const component = fixture.componentInstance;

    expect(component.isExpanded(1)).toBeFalse();
    component.toggleExpand(1);
    expect(component.isExpanded(1)).toBeTrue();
    expect(component.isExpanded(2)).toBeFalse();
  });
});
