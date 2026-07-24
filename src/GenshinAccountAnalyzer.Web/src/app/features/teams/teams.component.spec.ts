import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountAnalysis } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';
import { TeamsComponent } from './teams.component';

describe('TeamsComponent', () => {
  function seed(): void {
    const store = TestBed.inject(AccountStoreService);
    (store as unknown as { _analysis: { set: (v: AccountAnalysis) => void } })['_analysis'].set({
      uid: '1',
      averageBuildScore: 0,
      characters: [],
      teams: [
        {
          score: 88,
          reactionCore: { en: 'Vaporize', ru: 'Пар' },
          resonances: [
            { element: 'Cryo', name: { en: 'Shattering Ice', ru: 'Дробящий лёд' }, effect: { en: '', ru: '' } },
          ],
          members: [
            { characterId: 1, name: 'Kazuha', nameRu: 'Кадзуха', element: 'Anemo', buildScore: 90, role: 'Carry' },
            { characterId: 2, name: 'Wriothesley', nameRu: 'Ризли', element: 'Cryo', buildScore: 80, role: 'Sub-DPS' },
          ],
          reasons: [{ en: 'Reaction core: Vaporize', ru: 'Ядро реакции: Пар' }],
        },
      ],
    } as never);
  }

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [TeamsComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
    seed();
  });

  it('renders English team data by default', () => {
    const fixture = TestBed.createComponent(TeamsComponent);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Vaporize');
    expect(text).toContain('Kazuha');
    expect(text).toContain('Wriothesley');
    expect(text).toContain('Sub-DPS');
  });

  it('renders Russian team data (names, roles, resonance) when Russian is active', () => {
    TestBed.inject(LanguageService).set('ru');
    const fixture = TestBed.createComponent(TeamsComponent);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Пар');
    expect(text).toContain('Кадзуха');
    expect(text).toContain('Ризли');
    expect(text).toContain('Саб-ДПС');
    expect(text).toContain('Дробящий лёд');
  });
});
