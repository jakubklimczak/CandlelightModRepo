import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  //temporary
  private apiUrl = 'https://localhost:7275/api/UserAccess';

  constructor(private http: HttpClient) {}

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/SendLoginForm`, credentials);
  }

  register(user: { email: string; password: string; confirmPassword: string; username: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/SendRegistrationForm`, user);
  }
}
