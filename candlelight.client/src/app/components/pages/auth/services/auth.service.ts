import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../../../environment';
import { RegisterForm } from '../models/register-form.interface';
import { LoginForm } from '../models/login-form.interface';
import { UserProfileDto } from '../models/user-profile-dto.interface';
import { UserProfileDetails } from '../../user-profile/models/user-profile-details.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public authApiUrl = `${environment.apiUrl}/UserAccess`;
  public socialApiUrl = `${environment.apiUrl}/UserSocial`;  
  private userProfileSubject = new BehaviorSubject<UserProfileDto | null>(null);
  userProfile$ = this.userProfileSubject.asObservable();
  constructor(private http: HttpClient) {}

  public login(credentials: LoginForm): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.authApiUrl}/SendLoginForm`, credentials);
  }

  public register(form: RegisterForm): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.authApiUrl}/SendRegistrationForm`, form);
  }

  public getCurrentUserId(): Observable<{ id: string }> {
    return this.http.get<{ id: string }>(`${this.authApiUrl}/GetCurrentUserId`);
  }

  public getUserProfile(userId: string): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.socialApiUrl}/UserProfile/User/${userId}`);
  }

  public logout(): Observable<void> {
    return this.http.post<void>(`${this.authApiUrl}/Logout`, null);
  }

  public linkSteam(returnUrl: string) {
    this.http.get<{ url: string }>(`${this.authApiUrl}/LinkSteam?${returnUrl}`).subscribe((response: { url: string }) => {
      window.location.href = response.url;
    });
  }

  public updateUserProfile(profile: UserProfileDetails): void {
    this.userProfileSubject.next( {
      profileId: profile.id, 
      userId: profile.id,
      displayName: profile.displayName,
      bio: profile.bio,
      avatarFilename: profile.avatarFilename,
      createdAt: profile.createdAt,
      lastUpdatedAt: new Date(),
      backgroundColour: profile.backgroundColour
    } as UserProfileDto);
  }
}
