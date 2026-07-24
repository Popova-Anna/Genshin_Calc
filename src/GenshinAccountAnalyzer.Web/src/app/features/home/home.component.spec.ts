import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountAnalysis } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
  });

  it('shows the import panel when no account is loaded', () => {
    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('app-import-panel')).not.toBeNull();
  });

  it('shows the overview dashboard once an account is loaded', () => {
    const store = TestBed.inject(AccountStoreService);
    (store as unknown as { _analysis: { set: (v: AccountAnalysis) => void } })['_analysis'].set({
      uid: '1',
      averageBuildScore: 77.5,
      characters: [
        {
          characterId: 1,
          name: 'Kazuha',
          nameRu: 'Кадзуха',
          overallScore: 92,
          buildRating: { score: 92, tier: 'S' },
        } as never,
      ],
      teams: [{ reactionCore: { en: 'Vaporize', ru: 'Пар' }, score: 88, members: [] } as never],
    });

    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Kazuha');
    expect(text).toContain('Vaporize');
    expect(fixture.nativeElement.querySelector('app-import-panel')).toBeNull();
  });
});
