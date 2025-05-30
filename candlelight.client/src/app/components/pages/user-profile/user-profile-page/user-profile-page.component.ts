import { Component, OnInit } from '@angular/core';
import { UserProfileService } from '../services/user-profile.service';
import { UserProfileDetails } from '../models/user-profile-details.interface';

@Component({
  selector: 'app-user-profile-page',
  templateUrl: './user-profile-page.component.html',
  styleUrl: './user-profile-page.component.scss'
})
export class UserProfilePageComponent implements OnInit {
  profile!: UserProfileDetails;

  constructor(private readonly userProfileService: UserProfileService) {}

  ngOnInit() {
    this.userProfileService.
  }
}
