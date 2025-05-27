import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from '../pages/auth/services/auth.service';
import { UserProfileDto } from '../pages/auth/models/user-profile-dto.interface';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopbarComponent implements OnInit {
  isLoggedIn = false;
  currentUserId = '';
  userProfilePictureLink = '/android-chrome-512x512.png';

  constructor(
    private readonly authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ){};

  ngOnInit(): void {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkAuthState();
      });

    this.checkAuthState(); 
  }

  private checkAuthState(): void {
    const token = localStorage.getItem('authToken');
    if (!token) {
      this.isLoggedIn = false;
      this.cdr.markForCheck();
      return;
    }

    this.authService.getCurrentUserId().subscribe({
      next: (response) => {
        this.isLoggedIn = true;
        this.cdr.markForCheck();
        this.currentUserId = response.id;

        this.authService.getUserProfile(response.id).subscribe({
          next: (profile: UserProfileDto) => {
            this.userProfilePictureLink = profile.avatarFilename
              ? `/avatars/${profile.avatarFilename}`
              : '/android-chrome-512x512.png';
            this.cdr.markForCheck();
          },
          error: (e) => {
            console.warn(e);
          }
        });
      },
      error: () => {
        this.isLoggedIn = false;
        this.currentUserId = '';
        this.cdr.markForCheck();
      }
    });
  }
}


