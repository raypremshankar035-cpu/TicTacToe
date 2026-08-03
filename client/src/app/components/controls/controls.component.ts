import { Component, input, output } from '@angular/core';
import { GameMode } from '../../models/game.models';
 
@Component({
  selector: 'app-controls',
  standalone: true,
  templateUrl: './controls.component.html',
  styleUrl: './controls.component.scss',
})
export class ControlsComponent {
  selectedMode = input.required<GameMode>();
  hasActiveGame = input(false);
  canUndo = input(false);
  loading = input(false);
 
  modeChanged = output<GameMode>();
  newGame = output<void>();
  undo = output<void>();
  resetGame = output<void>();
  resetScoreboard = output<void>();
 
  onModeChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value as GameMode;
    this.modeChanged.emit(value);
  }
}