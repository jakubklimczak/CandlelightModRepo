import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, ReplaySubject } from "rxjs";
import { environment } from "../../../../../../environment";
import { GameDetailsDto } from "../models/game-details-dto";

@Injectable({
  providedIn: 'root',
})
export class SteamService {
    private ownedGames$ = new ReplaySubject<GameDetailsDto[]>(1);
    private hasLoaded = false;
    private gamesApiUrl = `${environment.apiUrl}/Game/`;
    private steamApiUrl = `${environment.apiUrl}/Steam/`;
    constructor(private http: HttpClient){}

    public getGameByAppId(appId: number): Observable<GameDetailsDto> {
        return this.http.get<GameDetailsDto>(`${this.gamesApiUrl}GetGame/${appId}`);
    }
}