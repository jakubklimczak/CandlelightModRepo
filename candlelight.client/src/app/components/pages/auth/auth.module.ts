import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginPageComponent } from './login-page/login-page.component';
import { RegistrationPageComponent } from './registration-page/registration-page.component';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { RouterModule } from '@angular/router';
import { SteamLoginPageComponent } from './steam-login-page/steam-login-page.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'
import { SteamLoginCallbackComponent } from './steam-login-page/steam-login-callback/steam-login-callback.component';

@NgModule({
  declarations: [
    LoginPageComponent, 
    RegistrationPageComponent, 
    SteamLoginPageComponent,
    SteamLoginCallbackComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    RouterModule,
    MatProgressSpinnerModule,
  ],
})
export class AuthModule {}
