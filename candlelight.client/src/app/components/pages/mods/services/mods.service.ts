import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PaginatedResponse } from "../../../../shared/models/paginated-result.model";
import { ModsListItemDto } from "../models/mods-list-item-dto.model";
import { Observable } from "rxjs";
import { environment } from "../../../../../../environment";
import { ModDetailsDto } from "../models/mod-details-dto.model";
import { PaginatedQuery } from "../../../../shared/models/paginated-query.model";
import { ModsSortingOptions } from "../enums/mods-sorting-options.enum";
import { ModUploadResponseDto } from "../models/mod-upload-response-dto.model";
import { GameDetailsDto } from "../../games/models/game-details-dto";
import { GamesSortingOptions } from "../../games/enums/games-sorting-options.enum";
import { ModVersion } from "../models/mod-version.model";

@Injectable({ providedIn: 'root' })
export class ModsService {
    constructor(private http: HttpClient) {}
    private apiUrl = `${environment.apiUrl}/Mod/`;
    private gameApiUrl = `${environment.apiUrl}/Game/`;

    public getModsForGame(
        gameId: string, 
        pagination: PaginatedQuery,
        searchTerm: string,
        showOnlyFavourites: boolean,
        sortBy: ModsSortingOptions
    ): Observable<PaginatedResponse<ModsListItemDto>> {
        return this.http.get<PaginatedResponse<ModsListItemDto>>(this.apiUrl + `GetModsByGameId`, {
            params: { gameId: gameId, page: pagination.page, pageSize: pagination.pageSize, sortBy: sortBy, showOnlyFavourites: showOnlyFavourites, searchTerm: searchTerm }
        });
    }

    public getModDetails(modId: string): Observable<ModDetailsDto> {
        return this.http.get<ModDetailsDto>(this.apiUrl + `GetModDetailsById`, { params: { modId } });
    }

    public getCurrentUserCreatedMods(): Observable<ModsListItemDto[]> {
        return this.http.get<ModsListItemDto[]>(`${this.apiUrl}CurrentUserCreatedMods`);
    }

    public getUserCreatedMods(userId: string): Observable<ModsListItemDto[]> {
        return this.http.get<ModsListItemDto[]>(`${this.apiUrl}UserCreatedMods/${userId}`);
    }

    public uploadNewMod(formData: FormData): Observable<ModUploadResponseDto> {
        return this.http.post<ModUploadResponseDto>(`${this.apiUrl}UploadNewMod`, formData);
    }

    public uploadNewModVersion(formData: FormData): Observable<ModUploadResponseDto> {
        return this.http.post<ModUploadResponseDto>(`${this.apiUrl}UploadNewModVersion`, formData);
    }

    public searchGames(term: string): Observable<PaginatedResponse<GameDetailsDto>> {
    return this.http.get<PaginatedResponse<GameDetailsDto>>(`${this.gameApiUrl}GetGamesFromDbPaginatedQuery`, {
        params: {
            page: 1,
            pageSize: 10,
            sortBy: GamesSortingOptions.Alphabetical,
            searchTerm: term
        }
    });
    }

    public getGameById(id: string): Observable<GameDetailsDto> {
        return this.http.get<GameDetailsDto>(`${this.gameApiUrl}GetGameFromDb/${id}`);
    }

    public getGameIdByModId(id: string): Observable<string> {
        return this.http.get<string>(`${this.gameApiUrl}GetGameIdByModId/${id}`);
    }

    public searchModsByGame(gameId: string, searchTerm: string): Observable<PaginatedResponse<ModsListItemDto>> {
        return this.http.get<PaginatedResponse<ModsListItemDto>>(`${this.apiUrl}GetModsByGameId`, {
            params: {
                gameId,
                page: 1,
                pageSize: 10,
                sortBy: ModsSortingOptions.Alphabetical,
                searchTerm,
            }
        });
    }

    public getModVersions(modId: string): Observable<ModVersion[]> {
        return this.http.get<ModVersion[]>(`${this.apiUrl}GetVersionsOfMod/${modId}`);
    }

}
