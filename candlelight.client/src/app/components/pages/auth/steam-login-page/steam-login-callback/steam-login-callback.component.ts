import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
@Component({
  selector: 'app-steam-login-callback',
  template:  `
    <div class="steam-login-container">
      <p>Redirecting to homepage...</p>
      <mat-progress-spinner mode="indeterminate"></mat-progress-spinner>
    </div>
  `,
})
export class SteamLoginCallbackComponent implements OnInit {
    constructor(private route: ActivatedRoute, private readonly router: Router) {}

    ngOnInit(): void {
        const token = this.route.snapshot.queryParamMap.get('token');
        if (token) {
            localStorage.setItem('authToken', token);
            this.router.navigate(['/']);
        }
    }
}
