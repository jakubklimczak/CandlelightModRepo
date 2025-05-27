import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthTokenService } from '../services/auth-token.service';

@Injectable({ providedIn: 'root' })
export class LoggedGuard implements CanActivate {
  constructor(private authTokenService: AuthTokenService, private router: Router) {}

  canActivate(): boolean {
    if (!this.authTokenService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }
}
