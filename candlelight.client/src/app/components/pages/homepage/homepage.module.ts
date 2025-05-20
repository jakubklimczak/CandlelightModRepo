import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterModule } from '@angular/router';
import { HeroPageComponent } from './hero/hero-page.component';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
  declarations: [HeroPageComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatCardModule,
    RouterModule,
    MatIconModule,
  ],
  providers: [],
  bootstrap: [HeroPageComponent],
})
export class HomepageModule {}
