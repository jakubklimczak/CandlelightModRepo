import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../services/user-profile.service';
import { UserProfileDetails } from '../models/user-profile-details.interface';
import { AuthService } from '../../auth/services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-profile-page',
  templateUrl: './user-profile-page.component.html',
  styleUrl: './user-profile-page.component.scss'
})
export class UserProfilePageComponent implements OnInit {
  profile!: UserProfileDetails;
  currentUserId!: string;
  profilePictureLink!: string;
  profileOwnerId!: string;

  constructor (
    private readonly route: ActivatedRoute, 
    private readonly userProfileService: UserProfileService, 
    private readonly authService: AuthService
  ) {}

  ngOnInit() {
    this.profileOwnerId = this.route.snapshot.paramMap.get('id')!;
    this.authService.getCurrentUserId().subscribe(response => {
      this.currentUserId = response.id;  
      this.initProfile();
    });
  }

  public initProfile(): void {
    const profileRequest$ = this.currentUserId === this.profileOwnerId
      ? this.userProfileService.getCurrentUserProfileDetails()
      : this.userProfileService.getUserProfileDetails(this.profileOwnerId);

    profileRequest$.subscribe(profile => {
      this.profile = profile;
      this.profilePictureLink = this.profile.avatarFilename
        ? `/avatars/${this.profile.avatarFilename}`
        : '/android-chrome-512x512.png';
    });
  }
}
