import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserProfileDetails } from '../models/user-profile-details.interface';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environment';
import { GameDetailsDto } from '../../games/models/game-details-dto';
import { ModsListItemDto } from '../../mods/models/mods-list-item-dto.model';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  constructor(private http: HttpClient) { }
  
  public apiUrl = `${environment.apiUrl}/UserSocial`;

  public getUserProfileDetails(userId: string): Observable<UserProfileDetails> {
    return this.http.get<UserProfileDetails>(`${this.apiUrl}/UserProfile/User/${userId}`);
  }

  public getCurrentUserProfileDetails(): Observable<UserProfileDetails> {
    return this.http.get<UserProfileDetails>(`${this.apiUrl}/UserProfile/CurrentUser`);
  }

  public updateUserProfile(formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/UserProfile/Update`, formData);
  }

  public getFavouriteGames(userId: string): Observable<GameDetailsDto[]> {
    return this.http.get<GameDetailsDto[]>(`${this.apiUrl}/FavouriteGames/${userId}`);
  }

  public getFavouriteMods(userId: string): Observable<ModsListItemDto[]> {
    return this.http.get<ModsListItemDto[]>(`${this.apiUrl}/FavouriteMods/${userId}`);
  }
}
