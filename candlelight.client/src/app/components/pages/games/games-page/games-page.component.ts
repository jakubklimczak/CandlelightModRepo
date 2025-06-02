import { Component, OnInit } from '@angular/core';
import { GameService } from '../services/game.service';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { GamesSortingOptions } from '../enums/games-sorting-options.enum';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { FormControl } from '@angular/forms';
import { GameDetailsDto } from '../models/game-details-dto';
import { AuthTokenService } from '../../../../shared/services/auth-token.service';

@Component({
  selector: 'app-games-page',
  templateUrl: './games-page.component.html',
  styleUrl: './games-page.component.scss',
})
export class GamesPageComponent implements OnInit {
  response?: PaginatedResponse<GameDetailsDto>;
  query: PaginatedQuery = { page: 1, pageSize: 50 };
  showOnlyFavourites = false;
  showOnlyOwned = false;
  showOnlyCustom = false;
  showOnlySteam = false;
  isLoggedIn = false;
  selectedSortOption = GamesSortingOptions.Alphabetical;
  GamesSortingOption = GamesSortingOptions;
  searchTerm: string | null = '';
  searchControl = new FormControl('');
  
  constructor(private gameService: GameService, private readonly authTokenService: AuthTokenService) {}

  ngOnInit(): void {
    this.loadGames();
    this.isLoggedIn = this.authTokenService.isLoggedIn();

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
      .getGames(
        this.query, 
        this.searchTerm ?? '', 
        this.showOnlyFavourites, 
        this.showOnlyOwned, 
        this.showOnlyCustom, 
        this.showOnlySteam, 
        this.selectedSortOption
      )
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
      this.showOnlyCustom = false;
      this.showOnlySteam = true;
    }
    this.loadGames();
  }

  public onSteamOnlyToggle(event: MatSlideToggleChange): void {
    this.showOnlySteam = event.checked;
    if (event.checked) {
      this.showOnlyCustom = false;
    }
    else {
      this.showOnlyOwned = false;
    }
    this.loadGames();
  }

  public onCustomOnlyToggle(event: MatSlideToggleChange): void {
    this.showOnlyCustom = event.checked;
    if (event.checked) {
      this.showOnlySteam = false;
      this.showOnlyOwned = false;
    }
    this.loadGames();
  }

  public resetFilters(): void {
    this.selectedSortOption = GamesSortingOptions.Alphabetical;
    this.showOnlyFavourites = false;
    this.showOnlyOwned = false;
    this.showOnlyCustom = false;
    this.showOnlySteam = false;
    this.searchControl.setValue('', { emitEvent: false });
    this.searchTerm = '';
    this.query.page = 1;
    this.loadGames();
  }
}
