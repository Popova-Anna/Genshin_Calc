import { TestBed } from '@angular/core/testing';
import { LanguageService } from './language.service';

describe('LanguageService', () => {
  beforeEach(() => localStorage.clear());

  it('defaults to English when nothing is stored', () => {
    const service = TestBed.inject(LanguageService);

    expect(service.language()).toBe('en');
  });

  it('toggle flips between en and ru and persists the choice', () => {
    const service = TestBed.inject(LanguageService);

    service.toggle();

    expect(service.language()).toBe('ru');
    expect(localStorage.getItem('gaa-lang')).toBe('ru');
  });

  it('pick returns the text for the active language', () => {
    const service = TestBed.inject(LanguageService);
    const text = { en: 'Upgrade weapon', ru: 'Улучшить оружие' };

    expect(service.pick(text)).toBe('Upgrade weapon');
    service.set('ru');
    expect(service.pick(text)).toBe('Улучшить оружие');
  });

  it('pickName falls back to the English name when no Russian name is set', () => {
    const service = TestBed.inject(LanguageService);
    service.set('ru');

    expect(service.pickName('Kazuha', null)).toBe('Kazuha');
    expect(service.pickName('Kazuha', 'Кадзуха')).toBe('Кадзуха');
  });

  it('pickName uses the English name when English is active even if Russian is set', () => {
    const service = TestBed.inject(LanguageService);

    expect(service.pickName('Kazuha', 'Кадзуха')).toBe('Kazuha');
  });

  it('priorityLabel translates the priority when Russian is active', () => {
    const service = TestBed.inject(LanguageService);
    service.set('ru');

    expect(service.priorityLabel('High')).toBe('Срочно');
    expect(service.priorityLabel('Medium')).toBe('Средне');
    expect(service.priorityLabel('Low')).toBe('Низко');
  });

  it('priorityLabel returns the raw value in English', () => {
    const service = TestBed.inject(LanguageService);

    expect(service.priorityLabel('High')).toBe('High');
  });
});
