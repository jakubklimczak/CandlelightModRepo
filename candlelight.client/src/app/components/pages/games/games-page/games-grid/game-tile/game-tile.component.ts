import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { GameDetailsDto } from '../../../models/game-details-dto';

@Component({
  selector: 'app-game-tile',
  templateUrl: './game-tile.component.html',
  styleUrl: './game-tile.component.scss',
})
export class GameTileComponent {
  @Input() game!: GameDetailsDto;

  constructor(private router: Router) {}

  public openMods(): void {
    this.router.navigate(['/mods'], {
      queryParams: { id: this.game.id },
    });
  }

  public openDetails(event: MouseEvent): void {
    event.stopPropagation();
    this.router.navigate(['/games/'], {
      queryParams: { id: this.game.id }
    });
  }

  public getGameName(game: GameDetailsDto): string {
    return game.name ?? 'Name missing!';
  }

  public getGameImage(game: GameDetailsDto): string {
    return game.isCustom ? 
      (game.headerImage ? '/custom-covers/' + game.headerImage : 'assets/default.png')
      : (game.headerImage ? game.headerImage : 'assets/default.png');
  }
}
