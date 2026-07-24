import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountAnalysis } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { RecommendationsComponent } from './recommendations.component';

describe('RecommendationsComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RecommendationsComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });

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
          recommendations: [
            { category: 'weapon', title: { en: 'Upgrade weapon', ru: '' }, detail: { en: 'x', ru: '' }, priority: 'High' },
            { category: 'talents', title: { en: 'Level talents', ru: '' }, detail: { en: 'y', ru: '' }, priority: 'Medium' },
          ],
        },
        {
          characterId: 2,
          name: 'Kazuha',
          nameRu: null,
          recommendations: [
            { category: 'crit', title: { en: 'Rebalance crit', ru: '' }, detail: { en: 'z', ru: '' }, priority: 'Low' },
          ],
        },
      ] as never,
    });
  });

  it('lists every recommendation sorted by priority, highest first', () => {
    const fixture = TestBed.createComponent(RecommendationsComponent);
    fixture.detectChanges();

    const priorities = fixture.componentInstance.rows().map((r) => r.rec.priority);
    expect(priorities).toEqual(['High', 'Medium', 'Low']);
  });

  it('filters by priority', () => {
    const fixture = TestBed.createComponent(RecommendationsComponent);
    const component = fixture.componentInstance;

    component.priorityFilter.set('High');
    fixture.detectChanges();

    expect(component.rows().length).toBe(1);
    expect(component.rows()[0].characterName).toBe('Zhongli');
  });
});
