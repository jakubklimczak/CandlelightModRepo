import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-hero-page',
  templateUrl: './hero-page.component.html',
  styleUrl: './hero-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeroPageComponent {}
