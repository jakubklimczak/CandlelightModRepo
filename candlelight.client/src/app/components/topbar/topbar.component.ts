import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { AuthService } from '../pages/auth/services/auth.service';
import { UserProfileDto } from '../pages/auth/models/user-profile-dto.interface';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopbarComponent implements OnInit {
  isLoggedIn = false;
  currentUserId = '';
  userProfilePictureLink = '/src/public/android-chrome-512x512.png';

  constructor(private readonly authService: AuthService){};

  ngOnInit(): void {
    this.getCurrentUser();
  }
  
  public getCurrentUser(): void {
    const token = localStorage.getItem('authToken');
    if (!token) {
      this.isLoggedIn = false;
      return;
    }

    this.authService.getCurrentUserId().subscribe({
      next: (response) => {
        this.isLoggedIn = true;
        this.currentUserId = response.id;

        this.authService.getUserProfile(response.id).subscribe({
          next: (profile: UserProfileDto) => {
            this.userProfilePictureLink = profile.avatarFilename
              ? `/assets/avatars/${profile.avatarFilename}`
              : '/src/public/android-chrome-512x512.png';
          },
          error: () => {
            console.warn('Could not load user profile');
          }
        });
      },
      error: () => {
        this.isLoggedIn = false;
        this.currentUserId = '';
      }
    });
  }
}


