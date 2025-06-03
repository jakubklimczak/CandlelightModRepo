import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, ReplaySubject } from "rxjs";
import { environment } from "../../../../../../environment";
import { GameInfoDto } from "../models/game-info-dto";

@Injectable({
  providedIn: 'root',
})
export class SteamService {
    private ownedGames$ = new ReplaySubject<GameInfoDto[]>(1);
    private hasLoaded = false;
    private gamesApiUrl = `${environment.apiUrl}/Game/`;
    private steamApiUrl = `${environment.apiUrl}/Steam/`;
    constructor(private http: HttpClient){}

    public getGameByAppId(appId: number): Observable<GameInfoDto> {
      return this.http.get<GameInfoDto>(`${this.gamesApiUrl}GetGame/${appId}`);
    }
}