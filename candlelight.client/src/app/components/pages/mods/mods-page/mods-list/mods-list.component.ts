import { Component, Input } from '@angular/core';
import { ModsListItemDto } from '../../models/mods-list-item-dto.model';

@Component({
  selector: 'app-mods-list',
  templateUrl: './mods-list.component.html',
  styleUrl: './mods-list.component.scss'
})
export class ModsListComponent {
  @Input() mods: ModsListItemDto[] = [];
}
