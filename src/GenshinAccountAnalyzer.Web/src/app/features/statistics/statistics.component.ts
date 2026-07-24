import { Component, computed, inject } from '@angular/core';
import { ApexChart, ApexOptions, ChartComponent } from 'ng-apexcharts';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';
import { elementColor, tierColor } from '../../core/theme-colors';
import { ElementType } from '../../core/models/enums';

const ELEMENT_ORDER: ElementType[] = [
  'Pyro',
  'Hydro',
  'Anemo',
  'Electro',
  'Dendro',
  'Cryo',
  'Geo',
  'Physical',
  'Unknown',
];
const TIER_ORDER = ['SS', 'S', 'A', 'B', 'C', 'D', 'F'] as const;

/** Account-wide statistics: element distribution, build-tier distribution, per-character scores. */
@Component({
  selector: 'app-statistics',
  imports: [ChartComponent],
  templateUrl: './statistics.component.html',
  styleUrl: './statistics.component.scss',
})
export class StatisticsComponent {
  private readonly store = inject(AccountStoreService);
  protected readonly language = inject(LanguageService);

  private readonly baseChart = (): Partial<ApexChart> => ({
    background: 'transparent',
    toolbar: { show: false },
    foreColor: '#9aa0c8',
  });

  readonly elementChart = computed<ApexOptions>(() => {
    const counts = new Map<ElementType, number>();
    for (const c of this.store.characters()) {
      counts.set(c.element, (counts.get(c.element) ?? 0) + 1);
    }
    const present = ELEMENT_ORDER.filter((e) => (counts.get(e) ?? 0) > 0);

    return {
      chart: { ...this.baseChart(), type: 'donut', height: 300 },
      series: present.map((e) => counts.get(e) ?? 0),
      labels: present,
      colors: present.map((e) => elementColor(e)),
      legend: { position: 'bottom' },
      dataLabels: { enabled: true },
    };
  });

  readonly tierChart = computed<ApexOptions>(() => {
    const counts = new Map<string, number>();
    for (const c of this.store.characters()) {
      counts.set(c.buildRating.tier, (counts.get(c.buildRating.tier) ?? 0) + 1);
    }
    const present = TIER_ORDER.filter((t) => (counts.get(t) ?? 0) > 0);

    return {
      chart: { ...this.baseChart(), type: 'bar', height: 300 },
      series: [{ name: 'Characters', data: present.map((t) => counts.get(t) ?? 0) }],
      xaxis: { categories: present as unknown as string[] },
      colors: present.map((t) => tierColor(t)),
      plotOptions: {
        bar: {
          distributed: true,
          borderRadius: 4,
          columnWidth: '55%',
        },
      },
      legend: { show: false },
      dataLabels: { enabled: true },
    };
  });

  readonly scoreChart = computed<ApexOptions>(() => {
    const chars = [...this.store.characters()].sort((a, b) => b.overallScore - a.overallScore);
    const names = chars.map((c) => this.language.pickName(c.name, c.nameRu));

    return {
      chart: { ...this.baseChart(), type: 'bar', height: Math.max(260, chars.length * 28) },
      series: [{ name: 'Build score', data: chars.map((c) => Math.round(c.overallScore)) }],
      xaxis: { categories: names, max: 100 },
      plotOptions: { bar: { horizontal: true, borderRadius: 3, distributed: true } },
      colors: chars.map((c) => tierColor(c.buildRating.tier)),
      legend: { show: false },
      dataLabels: { enabled: true },
    };
  });
}
