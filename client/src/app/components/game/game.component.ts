import { Component, OnInit, computed, inject } from '@angular/core';
import { GameStateService } from '../../services/game-state.service';
import { BoardComponent } from '../board/board.component';
import { StatusBannerComponent } from '../status-banner/status-banner.component';
import { ControlsComponent } from '../controls/controls.component';
import { MoveHistoryComponent } from '../move-history/move-history.component';
import { ScoreboardComponent } from '../scoreboard/scoreboard.component';
import { GameMode } from '../../models/game.models';
 
@Component({
  selector: 'app-game',
  standalone: true,
  imports: [BoardComponent, StatusBannerComponent, ControlsComponent, MoveHistoryComponent, ScoreboardComponent],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements OnInit {
  private readonly state = inject(GameStateService);
 
  readonly game = this.state.game;
  readonly loading = this.state.loading;
  readonly errorMessage = this.state.errorMessage;
  readonly selectedMode = this.state.selectedMode;
 
  readonly board = computed(() => this.state.board());
  readonly winningCells = computed(() => this.game()?.winningCells ?? []);
  readonly moves = computed(() => this.game()?.moveHistory ?? []);
  readonly scoreboard = computed(() => this.game()?.scoreboard ?? { xWins: 0, oWins: 0, draws: 0 });
  readonly boardDisabled = computed(() => !this.game() || this.game()!.status !== 'InProgress' || this.loading());
 
  ngOnInit(): void {
    this.state.startNewGame();
  }
 
  onModeChanged(mode: GameMode): void {
    this.state.setMode(mode);
    this.state.startNewGame(); // FIX: apply the mode change immediately instead of waiting for "New Game"
  }
 
  onNewGame(): void {
    this.state.startNewGame();
  }
 
  onCellClicked(event: { row: number; column: number }): void {
    this.state.playMove(event.row, event.column);
  }
 
  onUndo(): void {
    this.state.undo();
  }
 
  onResetGame(): void {
    this.state.resetGame();
  }
 
  onResetScoreboard(): void {
    this.state.resetScoreboard();
  }
 
  onDismissError(): void {
    this.state.clearError();
  }
}