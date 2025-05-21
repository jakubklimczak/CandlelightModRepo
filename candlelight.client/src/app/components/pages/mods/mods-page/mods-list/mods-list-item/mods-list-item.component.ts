import { Component, Input } from '@angular/core';
import { ModsListItemDto } from '../../../models/mods-list-item-dto.model';

@Component({
  selector: 'app-mods-list-item',
  templateUrl: './mods-list-item.component.html',
  styleUrl: './mods-list-item.component.scss'
})
export class ModsListItemComponent {
  @Input() mod!: ModsListItemDto;  
}
