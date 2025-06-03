import { NgModule } from '@angular/core';
import { GamesPageComponent } from './games-page/games-page.component';
import { GamesGridComponent } from './games-page/games-grid/games-grid.component';
import { GameTileComponent } from './games-page/games-grid/game-tile/game-tile.component';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../shared/shared.module';
import { GameDetailsPageComponent } from './game-details-page/game-details-page.component';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddNewGamePageComponent } from './add-new-game-page/add-new-game-page.component';
import { MatTabsModule } from '@angular/material/tabs'
import { MatFormFieldModule } from '@angular/material/form-field';
import { RouterModule } from '@angular/router';
import { AddSteamGameFormComponent } from './add-new-game-page/add-steam-game-form/add-steam-game-form.component';
import { AddCustomGameFormComponent } from './add-new-game-page/add-custom-game-form/add-custom-game-form.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    GamesPageComponent, 
    GamesGridComponent, 
    GameTileComponent, 
    GameDetailsPageComponent, 
    AddNewGamePageComponent, 
    AddSteamGameFormComponent, 
    AddCustomGameFormComponent,
  ],
  imports: [
    MatCardModule, 
    MatIconModule, 
    CommonModule, 
    SharedModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatFormFieldModule,
    RouterModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
})
export class GamesModule {}
