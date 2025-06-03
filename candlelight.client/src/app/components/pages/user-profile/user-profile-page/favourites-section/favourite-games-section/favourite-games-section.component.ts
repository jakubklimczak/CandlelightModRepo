import { Component, Input, OnInit } from '@angular/core';
import { UserProfileService } from '../../../services/user-profile.service';
import { GameInfoDto } from '../../../../games/models/game-info-dto';

@Component({
  selector: 'app-favourite-games-section',
  templateUrl: './favourite-games-section.component.html',
  styleUrls: ['./favourite-games-section.component.scss']
})
export class FavouriteGamesSectionComponent implements OnInit {
  @Input() userId!: string;
  favouriteGames: GameInfoDto[] = [];
  visibleRows = 1;
  readonly itemsPerRow = 5;

  constructor(private userProfileService: UserProfileService) {}

  ngOnInit(): void {
    this.userProfileService.getFavouriteGames(this.userId).subscribe(games => {
      this.favouriteGames = games;
    });
  }

  get visibleGames(): GameInfoDto[] {
    return this.favouriteGames.slice(0, this.visibleRows * this.itemsPerRow);
  }

  showMore(): void {
    this.visibleRows++;
  }

  collapse(): void {
    this.visibleRows = 1;
  }

  get canShowMore(): boolean {
    return this.visibleGames.length < this.favouriteGames.length;
  }

  get isExpanded(): boolean {
    return this.visibleRows > 1;
  }
}
