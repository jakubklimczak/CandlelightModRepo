import { Component, Input } from '@angular/core';
import { GameListItem } from '../../models/game-list-item.model';

@Component({
  selector: 'app-games-grid',
  templateUrl: './games-grid.component.html',
  styleUrl: './games-grid.component.scss',
})
export class GamesGridComponent {
  @Input() games: GameListItem[] = [];
}
