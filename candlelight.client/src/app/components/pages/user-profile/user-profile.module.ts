import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { UserProfilePageComponent } from './user-profile-page/user-profile-page.component';
import { FavouriteGamesPageComponent } from './favourite-games-page/favourite-games-page.component';
import { FavouriteModsPageComponent } from './favourite-mods-page/favourite-mods-page.component';
import { FavouritesSectionComponent } from './user-profile-page/favourites-section/favourites-section.component';
import { FavouriteGamesSectionComponent } from './user-profile-page/favourites-section/favourite-games-section/favourite-games-section.component';
import { FavouriteModssSectionComponent } from './user-profile-page/favourites-section/favourite-modss-section/favourite-modss-section.component';
import { FavouriteModsSectionComponent } from './user-profile-page/favourites-section/favourite-mods-section/favourite-mods-section.component';
import { FavouriteGameSectionItemComponent } from './user-profile-page/favourites-section/favourite-games-section/favourite-game-section-item/favourite-game-section-item.component';
import { FavouriteGamesSectionItemComponent } from './user-profile-page/favourites-section/favourite-games-section/favourite-games-section-item/favourite-games-section-item.component';
import { FavouriteModsSectionItemComponent } from './user-profile-page/favourites-section/favourite-mods-section/favourite-mods-section-item/favourite-mods-section-item.component';
import { EditUserProfilePageComponent } from './edit-user-profile-page/edit-user-profile-page.component';

@NgModule({
  declarations: [
    UserProfilePageComponent,
    FavouriteGamesPageComponent,
    FavouriteModsPageComponent,
    FavouritesSectionComponent,
    FavouriteGamesSectionComponent,
    FavouriteModssSectionComponent,
    FavouriteModsSectionComponent,
    FavouriteGameSectionItemComponent,
    FavouriteGamesSectionItemComponent,
    FavouriteModsSectionItemComponent,
    EditUserProfilePageComponent
  ],
  imports: [
    CommonModule,
    MatButtonModule,
  ],
})
export class UserProfileModule {}
