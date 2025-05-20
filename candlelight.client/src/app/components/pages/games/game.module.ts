import { NgModule } from '@angular/core';
import { GamesPageComponent } from './games-page/games-page.component';
import { GamesGridComponent } from './games-page/games-grid/games-grid.component';
import { GameTileComponent } from './games-page/games-grid/game-tile/game-tile.component';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { GameDetailsPageComponent } from './game-details-page/game-details-page.component';

@NgModule({
  declarations: [GamesPageComponent, GamesGridComponent, GameTileComponent, GameDetailsPageComponent],
  imports: [MatCardModule, MatIconModule, CommonModule, SharedModule],
})
export class GamesModule {}
