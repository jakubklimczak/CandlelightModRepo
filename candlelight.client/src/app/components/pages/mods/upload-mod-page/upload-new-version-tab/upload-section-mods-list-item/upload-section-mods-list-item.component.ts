import { Component, Input } from '@angular/core';
import { ModsListItemDto } from '../../../models/mods-list-item-dto.model';

@Component({
  selector: 'app-upload-section-mods-list-item',
  templateUrl: './upload-section-mods-list-item.component.html',
  styleUrl: './upload-section-mods-list-item.component.scss'
})
export class UploadSectionModsListItemComponent {
  @Input() mod!: ModsListItemDto;  
}
