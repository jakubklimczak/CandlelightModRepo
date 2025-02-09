import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HeroPageComponent } from './components/pages/homepage/hero/hero-page.component';
import { LoginPageComponent } from './components/pages/auth/login-page/login-page.component';
import { RegistrationPageComponent } from './components/pages/auth/registration-page/registration-page.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HeroPageComponent },
  { path: 'about', component: HeroPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegistrationPageComponent },];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
