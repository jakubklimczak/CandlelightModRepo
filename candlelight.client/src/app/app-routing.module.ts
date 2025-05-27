import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HeroPageComponent } from './components/pages/homepage/hero/hero-page.component';
import { LoginPageComponent } from './components/pages/auth/login-page/login-page.component';
import { RegistrationPageComponent } from './components/pages/auth/registration-page/registration-page.component';
import { GamesPageComponent } from './components/pages/games/games-page/games-page.component';
import { AboutPageComponent } from './components/pages/about-page/about-page.component';
import { ModsPageComponent } from './components/pages/mods/mods-page/mods-page.component';
import { ModDetailsPageComponent } from './components/pages/mods/mod-details-page/mod-details-page.component';
import { GameDetailsPageComponent } from './components/pages/games/game-details-page/game-details-page.component';
import { AddNewGamePageComponent } from './components/pages/games/add-new-game-page/add-new-game-page.component';
import { UploadModPageComponent } from './components/pages/mods/upload-mod-page/upload-mod-page.component';
import { SteamLoginPageComponent } from './components/pages/auth/steam-login-page/steam-login-page.component';
import { SteamLoginCallbackComponent } from './components/pages/auth/steam-login-page/steam-login-callback/steam-login-callback.component';
import { UserProfilePageComponent } from './components/pages/user-profile/user-profile-page/user-profile-page.component';
import { FavouriteGamesPageComponent } from './components/pages/user-profile/favourite-games-page/favourite-games-page.component';
import { FavouriteModsPageComponent } from './components/pages/user-profile/favourite-mods-page/favourite-mods-page.component';
import { NonLoggedGuard } from './shared/guards/non-logged.guard';
import { LoggedGuard } from './shared/guards/logged.guard';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HeroPageComponent },
  { path: 'about', component: AboutPageComponent },
  { path: 'login', component: LoginPageComponent, canActivate: [NonLoggedGuard] },
  { path: 'register', component: RegistrationPageComponent, canActivate: [NonLoggedGuard] },
  { path: 'steam-login', component: SteamLoginPageComponent, canActivate: [NonLoggedGuard] },  
  { path: 'steam-login/callback', component: SteamLoginCallbackComponent },
  { path: 'games', component: GamesPageComponent },  
  { path: 'games/add', component: AddNewGamePageComponent, canActivate: [LoggedGuard] },
  { path: 'games/:id', component: GameDetailsPageComponent },
  { path: 'mods', component: ModsPageComponent },  
  { path: 'mods/upload', component: UploadModPageComponent, canActivate: [LoggedGuard] },
  { path: 'mods/:id', component: ModDetailsPageComponent },  
  { path: 'profile/:id', component: UserProfilePageComponent },  
  { path: 'profile/:id/favourite-games', component: FavouriteGamesPageComponent, canActivate: [LoggedGuard] },  
  { path: 'profile/:id/favourite-mods', component: FavouriteModsPageComponent, canActivate: [LoggedGuard] },  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
