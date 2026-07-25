import { Routes } from '@angular/router';
import { hasDataGuard } from './core/guards/has-data.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/home.component').then((m) => m.HomeComponent),
    title: 'Genshin Account Analyzer',
  },
  {
    path: 'characters',
    loadComponent: () =>
      import('./features/characters/characters.component').then((m) => m.CharactersComponent),
    canActivate: [hasDataGuard],
    title: 'Characters — Genshin Account Analyzer',
  },
  {
    path: 'weapons',
    loadComponent: () => import('./features/weapons/weapons.component').then((m) => m.WeaponsComponent),
    canActivate: [hasDataGuard],
    title: 'Weapons — Genshin Account Analyzer',
  },
  {
    path: 'artifacts',
    loadComponent: () =>
      import('./features/artifacts/artifacts.component').then((m) => m.ArtifactsComponent),
    canActivate: [hasDataGuard],
    title: 'Artifacts — Genshin Account Analyzer',
  },
  {
    path: 'teams',
    loadComponent: () => import('./features/teams/teams.component').then((m) => m.TeamsComponent),
    canActivate: [hasDataGuard],
    title: 'Teams — Genshin Account Analyzer',
  },
  {
    path: 'statistics',
    loadComponent: () =>
      import('./features/statistics/statistics.component').then((m) => m.StatisticsComponent),
    canActivate: [hasDataGuard],
    title: 'Statistics — Genshin Account Analyzer',
  },
  {
    path: 'recommendations',
    loadComponent: () =>
      import('./features/recommendations/recommendations.component').then(
        (m) => m.RecommendationsComponent,
      ),
    canActivate: [hasDataGuard],
    title: 'Recommendations — Genshin Account Analyzer',
  },
  {
    path: 'rotation',
    loadComponent: () =>
      import('./features/rotation/rotation.component').then((m) => m.RotationComponent),
    title: 'Rotation Builder — Genshin Account Analyzer',
  },
  { path: '**', redirectTo: '' },
];
