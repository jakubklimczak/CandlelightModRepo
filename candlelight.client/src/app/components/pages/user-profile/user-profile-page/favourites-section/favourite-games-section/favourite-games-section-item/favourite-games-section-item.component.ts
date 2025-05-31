import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { GameDetailsDto } from '../../../../../games/models/game-details-dto';

@Component({
  selector: 'app-favourite-games-section-item',
  templateUrl: './favourite-games-section-item.component.html',
  styleUrl: './favourite-games-section-item.component.scss'
})
export class FavouriteGamesSectionItemComponent {
  @Input() game!: GameDetailsDto;

  constructor(private router: Router) {}

  public openMods(): void {
    this.router.navigate(['/mods'], {
      queryParams: { appId: this.game.id },
    });
  }

  public openDetails(event: MouseEvent): void {
    event.stopPropagation();
    this.router.navigate(['/games/' + this.game.id]);
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
