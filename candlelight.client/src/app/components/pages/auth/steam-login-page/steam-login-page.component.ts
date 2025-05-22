import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-steam-login-page',
  template:  `
    <div class="steam-login-container">
      <p>Redirecting to Steam for login...</p>
      <mat-progress-spinner mode="indeterminate"></mat-progress-spinner>
    </div>
  `,
  styleUrl: './steam-login-page.component.scss'
})
export class SteamLoginPageComponent implements OnInit {
  constructor(private readonly authService: AuthService) {}

  ngOnInit(): void {
    const returnUrl = `${window.location.origin}/steam-login/callback`;
    const steamLoginUrl = `${this.authService.authApiUrl}/SteamLogin?returnUrl=${encodeURIComponent(returnUrl)}`;
    window.location.href = steamLoginUrl;
  }
}
