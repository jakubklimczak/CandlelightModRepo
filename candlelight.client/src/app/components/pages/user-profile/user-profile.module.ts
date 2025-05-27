import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { UserProfilePageComponent } from './user-profile-page/user-profile-page.component';
import { FavouriteGamesPageComponent } from './favourite-games-page/favourite-games-page.component';
import { FavouriteModsPageComponent } from './favourite-mods-page/favourite-mods-page.component';

@NgModule({
  declarations: [
    UserProfilePageComponent,
    FavouriteGamesPageComponent,
    FavouriteModsPageComponent
  ],
  imports: [
    CommonModule,
    MatButtonModule,
  ],
})
export class UserProfileModule {}
