import { Component, Input, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/user-profile.service';
import { ModsListItemDto } from '../../../../mods/models/mods-list-item-dto.model';

@Component({
  selector: 'app-favourite-mods-section',
  templateUrl: './favourite-mods-section.component.html',
  styleUrl: './favourite-mods-section.component.scss'
})
export class FavouriteModsSectionComponent implements OnInit {
  @Input() userId!: string;
  favouriteMods: ModsListItemDto[] = [];
  visibleRows = 1;
  readonly itemsPerRow = 5;

  constructor(private userProfileService: UserProfileService) {}

  ngOnInit(): void {
    this.userProfileService.getFavouriteMods(this.userId).subscribe(mods => {
      this.favouriteMods = mods;
    });
  }

  get visibleMods(): ModsListItemDto[] {
    return this.favouriteMods.slice(0, this.visibleRows * this.itemsPerRow);
  }

  showMore(): void {
    this.visibleRows++;
  }

  collapse(): void {
    this.visibleRows = 1;
  }

  get canShowMore(): boolean {
    return this.visibleMods.length < this.favouriteMods.length;
  }

  get isExpanded(): boolean {
    return this.visibleRows > 1;
  }
}
