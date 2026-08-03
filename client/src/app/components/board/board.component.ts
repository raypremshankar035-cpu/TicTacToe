import { Component, input, output } from '@angular/core';
import { Player, WinningCell } from '../../models/game.models';
 
@Component({
  selector: 'app-board',
  standalone: true,
  templateUrl: './board.component.html',
  styleUrl: './board.component.scss',
})
export class BoardComponent {
  board = input.required<(Player | null)[][]>();
  winningCells = input<WinningCell[]>([]);
  disabled = input(false);
 
  cellClicked = output<{ row: number; column: number }>();
 
  isWinningCell(row: number, column: number): boolean {
    return this.winningCells().some((c) => c.row === row && c.column === column);
  }
 
  onCellClick(row: number, column: number, value: Player | null): void {
    if (this.disabled() || value !== null) return;
    this.cellClicked.emit({ row, column });
  }
}
