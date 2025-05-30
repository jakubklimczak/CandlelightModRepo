import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserProfileService } from '../services/user-profile.service';

@Component({
  selector: 'app-edit-user-profile-page',
  templateUrl: './edit-user-profile-page.component.html',
  styleUrl: './edit-user-profile-page.component.scss'
})
export class EditUserProfilePageComponent implements OnInit {
  profileForm!: FormGroup;
  userSteamId: string | null = null;

  constructor(private fb: FormBuilder, private readonly userProfileService: UserProfileService) {}

  ngOnInit() {
    this.userProfileService.getCurrentUserProfileDetails().subscribe(profile => {
      this.userSteamId = profile.steamId;
      this.profileForm = this.fb.group({
        displayName: [profile.displayName],
        bio: [profile.bio],
        backgroundColour: [profile.backgroundColour],
        email: [profile.email],
        phoneNumber: [profile.phoneNumber]
      });
    });
  }

  saveProfile() {
    this.userProfileService.updateProfile(this.profileForm.value).subscribe();
  }

  connectSteam() {
    window.location.href = '/auth/steam';
  }
}
