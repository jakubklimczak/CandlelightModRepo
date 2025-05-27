import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ReplaySubject } from "rxjs";
import { GameListItem } from "../models/game-list-item.model";

@Injectable({
  providedIn: 'root',
})
export class SteamService {
    private ownedGames$ = new ReplaySubject<GameListItem[]>(1);
    private hasLoaded = false;
    constructor(private http: HttpClient){}

}