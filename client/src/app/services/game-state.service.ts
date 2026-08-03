import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, finalize } from 'rxjs';
import { GameApiService } from './game-api.service';
import { GameMode, GameState, Player } from '../models/game.models';
 
const EMPTY_BOARD: (Player | null)[][] = [
  [null, null, null],
  [null, null, null],
  [null, null, null],
];
 
/**
 * Single source of frontend truth. Holds the last GameState returned by the
 * backend and exposes it (and derived values) as readonly signals. Every
 * mutation goes through the API first — this service never guesses at the
 * next board state, it only ever stores what the backend returned.
 */
@Injectable({ providedIn: 'root' })
export class GameStateService {
  private readonly api = inject(GameApiService);
 
  private readonly _game = signal<GameState | null>(null);
  private readonly _loading = signal(false);
  private readonly _errorMessage = signal<string | null>(null);
  private readonly _selectedMode = signal<GameMode>('TwoPlayer');
 
  readonly game = this._game.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly errorMessage = this._errorMessage.asReadonly();
  readonly selectedMode = this._selectedMode.asReadonly();
 
  readonly board = computed(() => this._game()?.board ?? EMPTY_BOARD);
 
  readonly statusMessage = computed(() => {
    const g = this._game();
    if (!g) return 'Choose a mode and start a new game.';
    if (g.status === 'Won') return `Player ${g.winner} wins!`;
    if (g.status === 'Draw') return "It's a draw!";
    return `Player ${g.currentPlayer}'s turn`;
  });
 
  setMode(mode: GameMode): void {
    this._selectedMode.set(mode);
  }
 
  startNewGame(): void {
    this.run(this.api.createGame({ gameMode: this._selectedMode() }));
  }
 
  playMove(row: number, column: number): void {
    const g = this._game();
    if (!g || g.status !== 'InProgress') return;
    if (g.board[row][column] !== null) return; // cell already taken — do not call the API
 
    this.run(this.api.makeMove(g.gameId, { player: g.currentPlayer, row, column }));
  }
 
  undo(): void {
    const g = this._game();
    if (!g || !g.canUndo) return;
    this.run(this.api.undo(g.gameId));
  }
 
  resetGame(): void {
    const g = this._game();
    if (!g) return;
    this.run(this.api.reset(g.gameId));
  }
 
  resetScoreboard(): void {
    this._loading.set(true);
    this._errorMessage.set(null);
 
    this.api.resetScoreboard()
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (scoreboard) => {
          const current = this._game();
          if (current) {
            this._game.set({ ...current, scoreboard });
          }
        },
        error: (err) => this.handleError(err),
      });
  }
 
  clearError(): void {
    this._errorMessage.set(null);
  }
 
  private run(source: Observable<GameState>): void {
    this._loading.set(true);
    this._errorMessage.set(null);
 
    source.pipe(finalize(() => this._loading.set(false))).subscribe({
      next: (state) => this._game.set(state),
      error: (err) => this.handleError(err),
    });
  }
 
  private handleError(err: any): void {
    const detail = err?.error?.detail ?? err?.message ?? 'Something went wrong. Please try again.';
    this._errorMessage.set(detail);
  }
}