import { Injectable, computed, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AccountApiService } from './account-api.service';
import { AccountAnalysis, CharacterAnalysis } from '../models/analysis.models';
import { ImportSource } from '../models/enums';

/**
 * Holds the currently loaded account analysis for the session. A single account is analyzed at a
 * time (no backend persistence yet — see the project roadmap for account history/multi-account
 * comparison), so a simple signal-based store is sufficient.
 */
@Injectable({ providedIn: 'root' })
export class AccountStoreService {
  private readonly _analysis = signal<AccountAnalysis | null>(null);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);

  readonly analysis = this._analysis.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();

  readonly hasData = computed(() => this._analysis() !== null);
  readonly characters = computed<CharacterAnalysis[]>(() => this._analysis()?.characters ?? []);

  constructor(private readonly api: AccountApiService) {}

  async load(file: File, source: ImportSource = 'Enka'): Promise<void> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const result = await firstValueFrom(this.api.analyze(file, source));
      this._analysis.set(result);
    } catch (err) {
      this._error.set(extractErrorMessage(err));
      throw err;
    } finally {
      this._loading.set(false);
    }
  }

  clear(): void {
    this._analysis.set(null);
    this._error.set(null);
  }

  findCharacter(id: number): CharacterAnalysis | undefined {
    return this.characters().find((c) => c.characterId === id);
  }
}

function extractErrorMessage(err: unknown): string {
  if (err && typeof err === 'object' && 'error' in err) {
    const body = (err as { error?: unknown }).error;
    if (body && typeof body === 'object' && 'detail' in body) {
      return String((body as { detail?: unknown }).detail);
    }
  }

  return 'Failed to import the account export. Make sure the file is a valid Enka.Network JSON export.';
}
