import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GameService } from '../services/game.service';
import { GameDetailsDto } from '../models/game-details-dto';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthTokenService } from '../../../../shared/services/auth-token.service';

@Component({
  selector: 'app-game-details-page',
  templateUrl: './game-details-page.component.html',
  styleUrl: './game-details-page.component.scss'
})
export class GameDetailsPageComponent implements OnInit {
  gameId!: string;
  game!: GameDetailsDto;
  isLoading = true;
  showFavouriteButton = false;
  isFavouritedByCurrentUser = false;
  isFavouriteStatusChanging = false;

  constructor(
    private readonly route: ActivatedRoute, 
    private readonly gameService: GameService, 
    private readonly snackBar: MatSnackBar,
    private readonly router: Router,
    private readonly authTokenService: AuthTokenService,
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.gameId = id;
        this.loadGameDetails();
      }
    });
  }

  public loadGameDetails(): void {
    this.isLoading = true;

    this.gameService.getGameDetails(this.gameId).subscribe({
      next: (data) => {
        this.game = data;
        this.checkFavouriteStatus();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.snackBar.open(
          'Game not found or an error occurred.',
          'Close',
          { duration: 4000, panelClass: ['snackbar-error'] }
        );
        this.router.navigate(['/games']);
      }
    });
  }

  public toggleFavourite(): void {
    if (!this.game) return;

    this.isFavouriteStatusChanging = true;

    const req = this.isFavouritedByCurrentUser
      ? this.gameService.removeGameFromFavourites(this.game.id)
      : this.gameService.addGameToFavourites(this.game.id);

    req.subscribe({
      next: () => {
        this.isFavouritedByCurrentUser = !this.isFavouritedByCurrentUser;
        this.game.favouriteCount += this.isFavouritedByCurrentUser ? 1 : -1;

        this.snackBar.open(
          this.isFavouritedByCurrentUser ? 'Added to favourites!' : 'Removed from favourites.',
          'Close',
          { duration: 3000 }
        );

        this.isFavouriteStatusChanging = false;
      },
      error: (err) => {
        const msg = err?.error ?? 'Something went wrong.';
        this.snackBar.open(msg, 'Close', { duration: 3000 });
        this.isFavouriteStatusChanging = false;
      }
    });
  }


  public checkFavouriteStatus(): void {
    if (!this.authTokenService.isLoggedIn()) {
      this.showFavouriteButton = false;
      return;
    }

    this.gameService.isGameFavourited(this.gameId).subscribe({
      next: (isFav) => {
        this.isFavouritedByCurrentUser = isFav;
        this.showFavouriteButton = true;
      },
      error: () => {
        this.showFavouriteButton = false;
      }
    });
  }
}
