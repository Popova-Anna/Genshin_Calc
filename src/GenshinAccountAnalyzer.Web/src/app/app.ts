import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AccountStoreService } from './core/services/account-store.service';
import { LanguageService } from './core/services/language.service';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly theme = inject(ThemeService);
  protected readonly language = inject(LanguageService);
  protected readonly store = inject(AccountStoreService);

  /** Links that only make sense once an account has been imported. */
  protected readonly accountNavLinks: { path: string; en: string; ru: string }[] = [
    { path: '/characters', en: 'Characters', ru: 'Персонажи' },
    { path: '/weapons', en: 'Weapons', ru: 'Оружие' },
    { path: '/artifacts', en: 'Artifacts', ru: 'Артефакты' },
    { path: '/teams', en: 'Teams', ru: 'Пачки' },
    { path: '/statistics', en: 'Statistics', ru: 'Статистика' },
    { path: '/recommendations', en: 'Recommendations', ru: 'Рекомендации' },
  ];

  /** Standalone tools that work without an imported account. */
  protected readonly toolNavLinks: { path: string; en: string; ru: string }[] = [
    { path: '/rotation', en: 'Rotation Builder', ru: 'Конструктор ротаций' },
  ];
}
