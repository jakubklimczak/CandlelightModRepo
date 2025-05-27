import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  exp: number;
  [key: string]: unknown;
}

@Injectable({
  providedIn: 'root'
})
export class AuthTokenService {
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  private hasToken(): boolean {
    return !!localStorage.getItem('authToken');
  }

  public isLoggedIn(): boolean {
    const token = localStorage.getItem('authToken');
    if (!token) return false;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      const now = Math.floor(Date.now() / 1000);
      return decoded.exp > now;
    } catch (e) {
      return false;
    }
  }

  public setToken(token: string): void {
    localStorage.setItem('authToken', token);
    this.isLoggedInSubject.next(true);
  }

  public clearToken(): void {
    localStorage.removeItem('authToken');
    this.isLoggedInSubject.next(false);
  }
}
