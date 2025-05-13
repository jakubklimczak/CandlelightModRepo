import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HeroPageComponent } from './components/pages/homepage/hero/hero-page.component';
import { LoginPageComponent } from './components/pages/auth/login-page/login-page.component';
import { RegistrationPageComponent } from './components/pages/auth/registration-page/registration-page.component';
import { GamesPageComponent } from './components/pages/games/games-page/games-page.component';
import { AboutPageComponent } from './components/pages/about-page/about-page.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HeroPageComponent },
  { path: 'about', component: AboutPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegistrationPageComponent },
  { path: 'games', component: GamesPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
