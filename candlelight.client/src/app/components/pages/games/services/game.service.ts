import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GameListItem } from '../models/game-list-item.model';
import { environment } from '../../../../../../environment';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { GamesSortingOptions } from '../enums/games-sorting-options.enum';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  constructor(private http: HttpClient) {}
  private apiUrl = `${environment.apiUrl}/Game/`;

  public getGames(
    pagination: PaginatedQuery,
    searchTerm: string,
    showOnlyFavourites: boolean,
    sortBy: GamesSortingOptions, 
  ): Observable<PaginatedResponse<GameListItem>> {
    return this.http.get<PaginatedResponse<GameListItem>>(
      this.apiUrl + 'GetGamesFromDbPaginatedQuery',
      {
        params: { page: pagination.page, pageSize: pagination.pageSize, searchTerm: searchTerm, showOnlyFavourites: showOnlyFavourites, sortBy: sortBy },
      },
    );
  }
}
