import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-favourites-section',
  templateUrl: './favourites-section.component.html',
  styleUrl: './favourites-section.component.scss'
})
export class FavouritesSectionComponent {
  @Input() userId!: string;
}
