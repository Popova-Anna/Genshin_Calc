import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { AccountStoreService } from './account-store.service';
import { AccountAnalysis } from '../models/analysis.models';

describe('AccountStoreService', () => {
  let httpMock: HttpTestingController;
  let store: AccountStoreService;

  const sampleFile = new File(['{}'], 'sample.json', { type: 'application/json' });
  const sampleAnalysis: AccountAnalysis = {
    uid: '123',
    characters: [],
    averageBuildScore: 0,
    teams: [],
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpMock = TestBed.inject(HttpTestingController);
    store = TestBed.inject(AccountStoreService);
  });

  afterEach(() => httpMock.verify());

  it('starts with no data and not loading', () => {
    expect(store.hasData()).toBeFalse();
    expect(store.loading()).toBeFalse();
    expect(store.characters()).toEqual([]);
  });

  it('load populates the analysis and clears the loading flag on success', async () => {
    const promise = store.load(sampleFile);

    const req = httpMock.expectOne((r) => r.url.includes('/api/account/analyze'));
    expect(req.request.method).toBe('POST');
    req.flush(sampleAnalysis);

    await promise;

    expect(store.hasData()).toBeTrue();
    expect(store.analysis()?.uid).toBe('123');
    expect(store.loading()).toBeFalse();
    expect(store.error()).toBeNull();
  });

  it('load sets an error message and rethrows on failure', async () => {
    const promise = store.load(sampleFile);

    const req = httpMock.expectOne((r) => r.url.includes('/api/account/analyze'));
    req.flush(
      { detail: 'The provided content is not valid Enka.Network JSON.' },
      { status: 400, statusText: 'Bad Request' },
    );

    await expectAsync(promise).toBeRejected();
    expect(store.error()).toContain('not valid Enka.Network JSON');
    expect(store.hasData()).toBeFalse();
  });

  it('clear resets analysis and error', async () => {
    const promise = store.load(sampleFile);
    httpMock.expectOne((r) => r.url.includes('/api/account/analyze')).flush(sampleAnalysis);
    await promise;

    store.clear();

    expect(store.hasData()).toBeFalse();
    expect(store.error()).toBeNull();
  });

  it('findCharacter looks up a character by id', async () => {
    const withCharacters: AccountAnalysis = {
      ...sampleAnalysis,
      characters: [{ characterId: 42, name: 'Test' } as never],
    };
    const promise = store.load(sampleFile);
    httpMock.expectOne((r) => r.url.includes('/api/account/analyze')).flush(withCharacters);
    await promise;

    expect(store.findCharacter(42)?.name).toBe('Test');
    expect(store.findCharacter(99)).toBeUndefined();
  });
});
