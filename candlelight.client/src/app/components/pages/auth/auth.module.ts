import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginPageComponent } from './login-page/login-page.component';
import { RegistrationPageComponent } from './registration-page/registration-page.component';

@NgModule({
  declarations: [
    LoginPageComponent, 
    RegistrationPageComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
})
export class AuthModule { }
