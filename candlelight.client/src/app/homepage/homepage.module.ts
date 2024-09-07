import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

import { HeroPageComponent } from './hero/hero-page.component';

@NgModule({
  declarations: [HeroPageComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatCardModule
  ],
  providers: [],
  bootstrap: [HeroPageComponent]
})
export class HomepageModule { }