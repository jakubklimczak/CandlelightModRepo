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

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HeroPageComponent },
  { path: 'about', component: AboutPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegistrationPageComponent },
  { path: 'games', component: GamesPageComponent },  
  { path: 'games/add', component: AddNewGamePageComponent },
  { path: 'games/:id', component: GameDetailsPageComponent },
  { path: 'mods', component: ModsPageComponent },  
  { path: 'mods/upload', component: UploadModPageComponent },
  { path: 'mods/:id', component: ModDetailsPageComponent },  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
