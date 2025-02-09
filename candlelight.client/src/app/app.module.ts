import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TopbarComponent } from './components/topbar/topbar.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HomepageModule } from './components/pages/homepage/homepage.module';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FooterComponent } from './components/footer/footer.component';
import { MatIconModule }  from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthModule } from './components/pages/auth/auth.module';

@NgModule({
  declarations: [
    AppComponent,
    TopbarComponent,
    FooterComponent,
  ],
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    BrowserModule,
    AppRoutingModule,
    HomepageModule,
    ReactiveFormsModule,
    AuthModule,
  ],
  providers: [provideHttpClient(), provideAnimationsAsync()],
  bootstrap: [AppComponent]
})
export class AppModule { }
