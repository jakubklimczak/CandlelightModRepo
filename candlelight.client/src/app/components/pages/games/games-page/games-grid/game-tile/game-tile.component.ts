import { Component, Input } from '@angular/core';
import { GameListItem } from '../../../models/game-list-item.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-game-tile',
  templateUrl: './game-tile.component.html',
  styleUrl: './game-tile.component.scss',
})
export class GameTileComponent {
  @Input() game!: GameListItem;

  constructor(private router: Router) {}

  public openMods(): void {
    this.router.navigate(['/mods'], {
      queryParams: { gameId: this.game.appId },
    });
  }

  public openDetails(event: MouseEvent): void {
    event.stopPropagation();
    this.router.navigate(['/games', this.game.appId]);
  }
}
