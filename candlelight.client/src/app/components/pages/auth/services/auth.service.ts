import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environment';
import { RegisterForm } from '../models/register-form.interface';
import { LoginForm } from '../models/login-form.interface';
import { UserProfileDto } from '../models/user-profile-dto.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public authApiUrl = `${environment.apiUrl}/UserAccess`;
  public socialApiUrl = `${environment.apiUrl}/UserSocial`;
  constructor(private http: HttpClient) {}

  public login(credentials: LoginForm): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.authApiUrl}/SendLoginForm`, credentials);
  }

  public register(form: RegisterForm): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.authApiUrl}/SendRegistrationForm`, form);
  }

  public steamLogin(returnUrl: string) {
    return this.http.post(`${this.authApiUrl}/SteamSteamLogin?returnUrl=${encodeURIComponent(returnUrl)}`, null)
  }

  public getCurrentUserId(): Observable<{ id: string }> {
    return this.http.get<{ id: string }>(`${this.authApiUrl}/GetCurrentUserId`);
  }

  public getUserProfile(userId: string): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.socialApiUrl}/UserProfile/${userId}`);
  }
}
