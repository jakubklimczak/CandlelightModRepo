import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { UserProfilePageComponent } from './user-profile-page/user-profile-page.component';
import { FavouriteGamesPageComponent } from './favourite-games-page/favourite-games-page.component';
import { FavouriteModsPageComponent } from './favourite-mods-page/favourite-mods-page.component';
import { FavouritesSectionComponent } from './user-profile-page/favourites-section/favourites-section.component';
import { FavouriteGamesSectionComponent } from './user-profile-page/favourites-section/favourite-games-section/favourite-games-section.component';
import { FavouriteModsSectionComponent } from './user-profile-page/favourites-section/favourite-mods-section/favourite-mods-section.component';
import { FavouriteGamesSectionItemComponent } from './user-profile-page/favourites-section/favourite-games-section/favourite-games-section-item/favourite-games-section-item.component';
import { FavouriteModsSectionItemComponent } from './user-profile-page/favourites-section/favourite-mods-section/favourite-mods-section-item/favourite-mods-section-item.component';
import { EditUserProfilePageComponent } from './edit-user-profile-page/edit-user-profile-page.component';
import { MatCardModule } from '@angular/material/card';
import { CreatedModsSectionComponent } from './user-profile-page/created-mods-section/created-mods-section.component';
import { CreatedModsSectionItemComponent } from './user-profile-page/created-mods-section/created-mods-section-item/created-mods-section-item.component';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ColorPickerModule } from 'ngx-color-picker';
import { SharedModule } from '../../../shared/shared.module';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';


@NgModule({
  declarations: [
    UserProfilePageComponent,
    FavouriteGamesPageComponent,
    FavouriteModsPageComponent,
    FavouritesSectionComponent,
    FavouriteGamesSectionComponent,
    FavouriteModsSectionComponent,
    FavouriteGamesSectionItemComponent,
    FavouriteModsSectionItemComponent,
    EditUserProfilePageComponent,
    CreatedModsSectionComponent,
    CreatedModsSectionItemComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    MatButtonModule,
    MatCardModule,
    RouterModule,
    MatIconModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatSlideToggleModule,
    ColorPickerModule,
  ],
})
export class UserProfileModule {}
