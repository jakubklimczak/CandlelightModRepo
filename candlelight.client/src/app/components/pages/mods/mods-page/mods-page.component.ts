import { Component, OnInit } from '@angular/core';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { ModsListItemDto } from '../models/mods-list-item-dto.model';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';
import { ActivatedRoute } from '@angular/router';
import { ModsService } from '../services/mods.service';
import { FormControl } from '@angular/forms';
import { ModsSortingOptions } from '../enums/mods-sorting-options.enum';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-mods-page',
  templateUrl: './mods-page.component.html',
  styleUrl: './mods-page.component.scss'
})
export class ModsPageComponent implements OnInit {
  response?: PaginatedResponse<ModsListItemDto>;
  query: PaginatedQuery = { page: 1, pageSize: 10 };
  gameId!: string;
  showOnlyFavourites = false;
  selectedSortOption = ModsSortingOptions.HighestRated;
  ModsSortingOption = ModsSortingOptions;
  searchTerm: string | null = '';
  searchControl = new FormControl('');
  
  constructor(private modService: ModsService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.gameId = params['appId'];
      this.query.page = 1;
      this.loadMods();
    });

    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => {
      this.searchTerm = value;
      this.loadMods();
    });
  }

  public loadMods(): void {
    this.modService.getModsForSteamGame(this.gameId, this.query, this.searchTerm ?? '', this.showOnlyFavourites, this.selectedSortOption).subscribe(res => {
      this.response = res;
    });
  }

  public onSortOptionChange(newValue: string) {
    this.selectedSortOption = newValue as ModsSortingOptions;
    this.loadMods();
  }

  public onPageChanged(event: { page: number; pageSize: number }): void {
    this.query.page = event.page;
    this.query.pageSize = event.pageSize;
    this.loadMods();
  }

  public onFavouritesToggle(event: MatSlideToggleChange): void {
    this.showOnlyFavourites = event.checked;
    this.loadMods();
  }

  public resetFilters(): void {
    this.selectedSortOption = ModsSortingOptions.HighestRated;
    this.showOnlyFavourites = false;
    this.searchControl.setValue('', { emitEvent: false });
    this.searchTerm = '';
    this.query.page = 1;
    this.loadMods();
  }
}
