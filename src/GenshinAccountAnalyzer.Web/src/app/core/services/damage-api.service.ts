import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Rotation, RotationResult } from '../models/damage.models';

/** Talks to the GenshinAccountAnalyzer.Api damage endpoints. */
@Injectable({ providedIn: 'root' })
export class DamageApiService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/damage`;

  constructor(private readonly http: HttpClient) {}

  /** Evaluates a full rotation (sequence of hits/reaction procs). */
  calculateRotation(rotation: Rotation): Observable<RotationResult> {
    return this.http.post<RotationResult>(`${this.baseUrl}/rotation`, rotation);
  }
}
