import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountAnalysis } from '../../core/models/analysis.models';
import { AccountStoreService } from '../../core/services/account-store.service';
import { StatisticsComponent } from './statistics.component';

describe('StatisticsComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [StatisticsComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });

    const store = TestBed.inject(AccountStoreService);
    (store as unknown as { _analysis: { set: (v: AccountAnalysis) => void } })['_analysis'].set({
      uid: '1',
      averageBuildScore: 0,
      teams: [],
      characters: [
        { characterId: 1, name: 'A', nameRu: null, element: 'Pyro', overallScore: 90, buildRating: { score: 90, tier: 'S' } },
        { characterId: 2, name: 'B', nameRu: null, element: 'Pyro', overallScore: 60, buildRating: { score: 60, tier: 'B' } },
        { characterId: 3, name: 'C', nameRu: null, element: 'Hydro', overallScore: 75, buildRating: { score: 75, tier: 'A' } },
      ] as never,
    });
  });

  it('groups the element distribution by count', () => {
    const fixture = TestBed.createComponent(StatisticsComponent);
    fixture.detectChanges();

    const chart = fixture.componentInstance.elementChart();
    expect(chart.labels).toEqual(['Pyro', 'Hydro']);
    expect(chart.series).toEqual([2, 1]);
  });

  it('groups the tier distribution by count, highest tier first', () => {
    const fixture = TestBed.createComponent(StatisticsComponent);
    fixture.detectChanges();

    const chart = fixture.componentInstance.tierChart();
    expect(chart.xaxis?.categories).toEqual(['S', 'A', 'B']);
    expect((chart.series as { data: number[] }[])[0].data).toEqual([1, 1, 1]);
  });

  it('sorts the per-character score chart by overall score descending', () => {
    const fixture = TestBed.createComponent(StatisticsComponent);
    fixture.detectChanges();

    const chart = fixture.componentInstance.scoreChart();
    expect(chart.xaxis?.categories).toEqual(['A', 'C', 'B']);
    expect((chart.series as { data: number[] }[])[0].data).toEqual([90, 75, 60]);
  });
});
