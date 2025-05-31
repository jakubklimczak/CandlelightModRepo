import { Component, Input } from '@angular/core';
import { ModsListItemDto } from '../../../../mods/models/mods-list-item-dto.model';

@Component({
  selector: 'app-created-mods-section-item',
  templateUrl: './created-mods-section-item.component.html',
  styleUrl: './created-mods-section-item.component.scss'
})
export class CreatedModsSectionItemComponent {
  @Input() mod!: ModsListItemDto;
}
