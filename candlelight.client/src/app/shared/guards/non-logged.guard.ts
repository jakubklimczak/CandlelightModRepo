import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthTokenService } from '../services/auth-token.service';

@Injectable({ providedIn: 'root' })
export class NonLoggedGuard implements CanActivate {
  constructor(private authTokenService: AuthTokenService, private router: Router) {}

  canActivate(): boolean {
    if (this.authTokenService.isLoggedIn()) {
      this.router.navigate(['/']);
      return false;
    }
    return true;
  }
}