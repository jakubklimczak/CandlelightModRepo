import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopbarComponent {
  isLoggedIn = false;
  //TODO: make it a placeholder by default
  userProfilePictureLink = '';
}
