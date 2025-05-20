import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PaginatedResponse } from "../../../../shared/models/paginated-result.model";
import { ModsListItemDto } from "../models/mods-list-item-dto.model";
import { Observable } from "rxjs";
import { environment } from "../../../../../../environment";
import { ModDetailsDto } from "../models/mod-details-dto.model";

@Injectable({ providedIn: 'root' })
export class ModsService {
    constructor(private http: HttpClient) {}
    private apiUrl = `${environment.apiUrl}/Mod/`;

    public getModsForSteamGame(appId: string, page: number, pageSize: number): Observable<PaginatedResponse<ModsListItemDto>> {
        return this.http.get<PaginatedResponse<ModsListItemDto>>(this.apiUrl + `GetModsBySteamAppId`, {
            params: { appId, page, pageSize }
        });
    }

    public getModDetails(modId: string): Observable<ModDetailsDto> {
        return this.http.get<ModDetailsDto>(this.apiUrl + `GetModDetailsById`, { params: { modId } });
    }
}
