import { Component, input } from '@angular/core';
import { Move } from '../../models/game.models';
 
@Component({
  selector: 'app-move-history',
  standalone: true,
  templateUrl: './move-history.component.html',
  styleUrl: './move-history.component.scss',
})
export class MoveHistoryComponent {
  moves = input<Move[]>([]);
}
