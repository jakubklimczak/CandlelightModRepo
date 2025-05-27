import { Component, Input } from '@angular/core';
import { GameDetailsDto } from '../../models/game-details-dto';

@Component({
  selector: 'app-games-grid',
  templateUrl: './games-grid.component.html',
  styleUrl: './games-grid.component.scss',
})
export class GamesGridComponent {
  @Input() games: GameDetailsDto[] = [];
}
