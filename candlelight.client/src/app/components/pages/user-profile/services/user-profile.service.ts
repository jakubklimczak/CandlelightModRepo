import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserProfileDetails } from '../models/user-profile-details.interface';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  constructor(private http: HttpClient) { }

  public getUserProfileDetails(userId: string): UserProfileDetails {
    //TODO: implement
  }

  public getCurrentUserProfileDetails(): UserProfileDetails {
    //TODO: implement
  }

  public updateProfile() {
    //TODO: implement
  }
}
