import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateGameRequest, GameState, MoveRequest, Scoreboard } from '../models/game.models';
 
@Injectable({ providedIn: 'root' })
export class GameApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;
 
  createGame(request: CreateGameRequest): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/api/games`, request);
  }
 
  getGame(gameId: string): Observable<GameState> {
    return this.http.get<GameState>(`${this.baseUrl}/api/games/${gameId}`);
  }
 
  makeMove(gameId: string, request: MoveRequest): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/api/games/${gameId}/moves`, request);
  }
 
  undo(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/api/games/${gameId}/undo`, {});
  }
 
  reset(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/api/games/${gameId}/reset`, {});
  }
 
  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.baseUrl}/api/scoreboard`);
  }
 
  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.baseUrl}/api/scoreboard/reset`, {});
  }
}
