import { Component, input } from '@angular/core';
import { GameStatus, Player } from '../../models/game.models';
 
@Component({
  selector: 'app-status-banner',
  standalone: true,
  templateUrl: './status-banner.component.html',
  styleUrl: './status-banner.component.scss',
})
export class StatusBannerComponent {
  status = input.required<GameStatus>();
  currentPlayer = input.required<Player>();
  winner = input<Player | null>(null);
}
