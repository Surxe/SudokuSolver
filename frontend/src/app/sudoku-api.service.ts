import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { PuzzleResponse, SolveRequest, SolveResponse } from './models/puzzle.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SudokuApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getRandomPuzzle(): Observable<PuzzleResponse> {
    return this.http.get<PuzzleResponse>(`${this.apiUrl}/puzzle`).pipe(
      catchError(this.handleError)
    );
  }

  solvePuzzle(board: number[][]): Observable<SolveResponse> {
    const request: SolveRequest = { board };
    return this.http.post<SolveResponse>(`${this.apiUrl}/solve`, request).pipe(
      catchError(this.handleError)
    );
  }

  solvePuzzleStream(board: number[][]): EventSource {
    const encodedBoard = encodeURIComponent(JSON.stringify(board));
    return new EventSource(`${this.apiUrl}/solve/stream?board=${encodedBoard}`);
  }

  private handleError(error: any): Observable<never> {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}
