import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  private apiUrl = `${environment.apiUrl}/UserAccess`;
  constructor(private http: HttpClient) { }

  //TODO: replace any w dto
  public login(credentials: { userEmail: string; passwordString: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http.post(`${this.apiUrl}/SendLoginForm`, credentials, { headers });
  }

  //TODO: replace any w dto
  public register(user: { username: string; email: string; password: string; confirmPassword: string; }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    
    const requestBody = {
      userName: user.username, 
      userEmail: user.email,
      passwordString: user.password,
      confirmPasswordString: user.confirmPassword
    };

    return this.http.post(`${this.apiUrl}/SendRegistrationForm`, requestBody, { headers });
  }
}
