import { Component, Input } from '@angular/core';
import { GameInfoDto } from '../../models/game-info-dto';

@Component({
  selector: 'app-games-grid',
  templateUrl: './games-grid.component.html',
  styleUrl: './games-grid.component.scss',
})
export class GamesGridComponent {
  @Input() games: GameInfoDto[] = [];
}
