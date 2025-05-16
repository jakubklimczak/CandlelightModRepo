import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PaginatedResponse } from "../../../../shared/models/paginated-result.model";
import { ModsListItemDto } from "../models/mods-list-item-dto.model";
import { Observable } from "rxjs";
import { environment } from "../../../../../../environment";

@Injectable({ providedIn: 'root' })
export class ModsService {
    constructor(private http: HttpClient) {}
    private apiUrl = `${environment.apiUrl}/Mod/`;

    public getModsForGame(gameId: string, page: number, pageSize: number): Observable<PaginatedResponse<ModsListItemDto>> {
        return this.http.get<PaginatedResponse<ModsListItemDto>>(this.apiUrl + `GetModsByGameId`, {
            params: { gameId, page, pageSize }
        });
    }
}
