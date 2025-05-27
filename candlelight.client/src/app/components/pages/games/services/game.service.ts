import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environment';
import { PaginatedQuery } from '../../../../shared/models/paginated-query.model';
import { PaginatedResponse } from '../../../../shared/models/paginated-result.model';
import { GamesSortingOptions } from '../enums/games-sorting-options.enum';
import { GameDetailsDto } from '../models/game-details-dto';

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
    showOnlyOwned: boolean,
    showOnlyCustom: boolean,
    showOnlySteam: boolean,
    sortBy: GamesSortingOptions, 
  ): Observable<PaginatedResponse<GameDetailsDto>> {
    return this.http.get<PaginatedResponse<GameDetailsDto>>(
      this.apiUrl + 'GetGamesFromDbPaginatedQuery',
      {
        params: { 
          page: pagination.page, 
          pageSize: pagination.pageSize, 
          searchTerm: searchTerm, 
          showOnlyFavourites: showOnlyFavourites, 
          showOnlyOwned: showOnlyOwned, 
          showOnlyCustom: showOnlyCustom,
          showOnlySteam: showOnlySteam,
          sortBy: sortBy 
        },
      },
    );
  }

  public uploadCustomGame(formData: FormData): Observable<string> {
    return this.http.post<string>(this.apiUrl + 'AddCustom', formData);
  }
}
