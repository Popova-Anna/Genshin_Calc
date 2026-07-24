import { TestBed } from '@angular/core/testing';
import { ThemeService } from './theme.service';

describe('ThemeService', () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-bs-theme');
  });

  it('defaults to dark when nothing is stored and the system prefers dark', () => {
    spyOn(window, 'matchMedia').and.returnValue({ matches: true } as MediaQueryList);
    const service = TestBed.inject(ThemeService);

    expect(service.theme()).toBe('dark');
    expect(document.documentElement.getAttribute('data-bs-theme')).toBe('dark');
  });

  it('toggle flips between dark and light and persists the choice', () => {
    const service = TestBed.inject(ThemeService);
    service.set('dark');

    service.toggle();

    expect(service.theme()).toBe('light');
    expect(document.documentElement.getAttribute('data-bs-theme')).toBe('light');
    expect(localStorage.getItem('gaa-theme')).toBe('light');
  });

  it('a fresh service instance reads the previously persisted theme', () => {
    TestBed.inject(ThemeService).set('light');

    TestBed.resetTestingModule();
    const restored = TestBed.inject(ThemeService);

    expect(restored.theme()).toBe('light');
  });
});
