import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserProfileService } from '../services/user-profile.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'app-edit-user-profile-page',
  templateUrl: './edit-user-profile-page.component.html',
  styleUrl: './edit-user-profile-page.component.scss'
})
export class EditUserProfilePageComponent implements OnInit {
  profileForm!: FormGroup;
  hasSteamId = false;
  selectedFile?: File;
  profilePictureLink!: string;

  constructor(
    private fb: FormBuilder,
    private userProfileService: UserProfileService,
    private readonly snackBar: MatSnackBar,
    private readonly authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.initProfileInfo();
  }

  public initProfileInfo(): void {
    this.userProfileService.getCurrentUserProfileDetails().subscribe(profile => {
      this.hasSteamId = !!profile.steamId;
      this.profilePictureLink = profile.avatarFilename
        ? `/avatars/${profile.avatarFilename}?t=${Date.now()}`
        : '/android-chrome-512x512.png';
      this.profileForm = this.fb.group({
        displayName: [profile.displayName, [Validators.minLength(6)]],
        bio: [profile.bio, [Validators.maxLength(200)]],
        backgroundColour: [profile.backgroundColour || '#363636', [Validators.maxLength(30)]],
        favouritesVisible: [profile.favouritesVisible ?? false],
      });
      this.authService.updateUserProfile(profile); 
    });
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    const file = target.files?.[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  public submit(): void {
    const formData = new FormData();
    formData.append('displayName', this.profileForm.value.displayName);
    formData.append('bio', this.profileForm.value.bio);
    formData.append('backgroundColour', this.profileForm.value.backgroundColour);
    formData.append('favouritesVisible', String(this.profileForm.value.favouritesVisible));
    if (this.selectedFile) {
      formData.append('avatar', this.selectedFile);
    }

    this.submitProfileUpdate(formData);
  }

  public submitProfileUpdate(formData: FormData): void {
    this.userProfileService.updateUserProfile(formData).subscribe({
      next: () => {
        this.snackBar.open('Profile updated successfully!', 'Close', {
          duration: 3000,
          panelClass: ['snackbar-success']
        });
        this.initProfileInfo();
      },
      error: () => {
        this.snackBar.open('Failed to update profile. Please try again.', 'Close', {
          duration: 3000,
          panelClass: ['snackbar-error']
        });
      }
    });
  }

  public linkSteam(): void {
    this.authService.linkSteam(window.location.origin);
  }
}
