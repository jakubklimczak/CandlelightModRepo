import { Component, Input } from '@angular/core';
import { ModsListItemDto } from '../../../../../mods/models/mods-list-item-dto.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-favourite-mods-section-item',
  templateUrl: './favourite-mods-section-item.component.html',
  styleUrl: './favourite-mods-section-item.component.scss'
})
export class FavouriteModsSectionItemComponent {
  @Input() mod!: ModsListItemDto;

  constructor(private router: Router) {}

  public openDetails(event: MouseEvent): void {
    event.stopPropagation();
    this.router.navigate(['/mods/' + this.mod.id]);
  }
}
