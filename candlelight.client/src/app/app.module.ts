import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TopbarComponent } from './components/topbar/topbar.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HomepageModule } from './components/pages/homepage/homepage.module';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FooterComponent } from './components/footer/footer.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthModule } from './components/pages/auth/auth.module';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { GamesModule } from './components/pages/games/game.module';
import { AboutPageComponent } from './components/pages/about-page/about-page.component';
import { SharedModule } from './shared/shared.module';
import { ModsModule } from './components/pages/mods/mods.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { authInterceptor } from './shared/services/auth.interceptor';
import { UserProfileModule } from './components/pages/user-profile/user-profile.module';

@NgModule({
  declarations: [
    AppComponent,
    TopbarComponent,
    FooterComponent,
    AboutPageComponent,
  ],
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    BrowserModule,
    BrowserAnimationsModule, 
    AppRoutingModule,
    HomepageModule,
    ReactiveFormsModule,
    AuthModule,
    GamesModule,
    SharedModule,
    MatSnackBarModule,
    ModsModule,
    UserProfileModule,
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])), 
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
