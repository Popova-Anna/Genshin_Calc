import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AccountAnalysis } from '../models/analysis.models';
import { ImportSource } from '../models/enums';

/** Talks to the GenshinAccountAnalyzer.Api account endpoints. */
@Injectable({ providedIn: 'root' })
export class AccountApiService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/account`;

  constructor(private readonly http: HttpClient) {}

  /** Imports and fully analyzes a raw export file, returning the account analysis. */
  analyze(file: File, source: ImportSource = 'Enka'): Observable<AccountAnalysis> {
    return this.http.post<AccountAnalysis>(`${this.baseUrl}/analyze?source=${source}`, file, {
      headers: { 'Content-Type': 'application/json' },
    });
  }

  /** Imports, analyzes and renders a raw export file as a self-contained HTML report. */
  report(file: File, source: ImportSource = 'Enka'): Observable<string> {
    return this.http.post(`${this.baseUrl}/report?source=${source}`, file, {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text',
    });
  }
}
