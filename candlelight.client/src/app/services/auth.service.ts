import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  private apiUrl = `${environment.apiUrl}/UserAccess`;

  constructor(private http: HttpClient) { }


  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/SendLoginForm`, credentials);
  }

  register(user: { email: string; password: string; confirmPassword: string; username: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/SendRegistrationForm`, user);
  }
}
