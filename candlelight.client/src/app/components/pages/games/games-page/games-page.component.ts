import { Component, OnInit } from '@angular/core';
import { GameListItem } from '../models/game-list-item.model';
import { GameService } from '../services/game.service';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { GamesSortingOptions } from '../enums/games-sorting-options.enum';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-games-page',
  templateUrl: './games-page.component.html',
  styleUrl: './games-page.component.scss',
})
export class GamesPageComponent implements OnInit {
  response?: PaginatedResponse<GameListItem>;
  query: PaginatedQuery = { page: 1, pageSize: 50 };
  showOnlyFavourites = false;
  showOnlyOwned = false;
  selectedSortOption = GamesSortingOptions.Alphabetical;
  GamesSortingOption = GamesSortingOptions;
  searchTerm: string | null = '';
  searchControl = new FormControl('');
  
  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.loadGames();

    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => {
      this.searchTerm = value;
      this.loadGames();
    });
  }

  public onSortOptionChange(newValue: string) {
    this.selectedSortOption = newValue as GamesSortingOptions;
    this.loadGames();
  }

  public loadGames(): void {
    this.gameService
      .getGames(this.query, this.searchTerm ?? '', this.showOnlyFavourites, this.showOnlyOwned, this.selectedSortOption)
      .subscribe((games) => {
        this.response = games;
      });
  }

  public onPageChanged(event: { page: number; pageSize: number }): void {
    this.query.page = event.page;
    this.query.pageSize = event.pageSize;
    this.loadGames();
  }

  public onFavouritesToggle(event: MatSlideToggleChange): void {
    this.showOnlyFavourites = event.checked;
    if (event.checked) {
      this.showOnlyOwned = false;
    }
    this.loadGames();
  }

  public onOwnedToggle(event: MatSlideToggleChange): void {
    this.showOnlyOwned = event.checked;
    if (event.checked) {
      this.showOnlyFavourites = false;
    }
    this.loadGames();
  }

  public resetFilters(): void {
    this.selectedSortOption = GamesSortingOptions.Alphabetical;
    this.showOnlyFavourites = false;
    this.showOnlyOwned = false;
    this.searchControl.setValue('', { emitEvent: false });
    this.searchTerm = '';
    this.query.page = 1;
    this.loadGames();
  }
}
