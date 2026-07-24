// Global Jasmine hook (matched by the default *.spec.ts include glob so Karma bundles it
// automatically). localStorage is real in Karma's browser and persists across spec files within
// the same run; clearing it before every test prevents theme/language state leaking between specs.
beforeEach(() => localStorage.clear());
