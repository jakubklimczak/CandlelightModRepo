import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ModDetailsDto } from '../models/mod-details-dto.model';
import { ModsService } from '../services/mods.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthTokenService } from '../../../../shared/services/auth-token.service';

@Component({
  selector: 'app-mod-details-page',
  templateUrl: './mod-details-page.component.html',
  styleUrl: './mod-details-page.component.scss'
})
export class ModDetailsPageComponent implements OnInit {
  modId!: string;
  details!: ModDetailsDto;
  isLoading = true;
  showFavouriteButton = false;
  isFavourited = false;
  isFavouriteStatusChanging = false;
  images: string[] = [];

  constructor(
    private route: ActivatedRoute, 
    private modsService: ModsService,
    private snackBar: MatSnackBar,
    private router: Router,
    private authTokenService: AuthTokenService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(p => this.modId = p["id"]);
    this.isLoading = true;

    this.modsService.getModDetails(this.modId).subscribe({
      next: (data) => {
        this.details = data;

        if (!this.authTokenService.isLoggedIn()) {
          this.showFavouriteButton = false;
          this.isLoading = false;
          return;
        }

        this.modsService.isModFavourited(this.modId).subscribe({
          next: (isFav) => {
            this.isFavourited = isFav;
            this.showFavouriteButton = true;
            this.isLoading = false;
          },
          error: () => {
            this.showFavouriteButton = false;
            this.isLoading = false;
          }
        });
      },
      error: () => {
        this.snackBar.open('Mod not found or error occurred.', 'Close', { duration: 4000 });
        this.router.navigate(['/games']);
      }
    });
  }

  public getDetails(id: string): void {
    this.modsService.getModDetails(id).subscribe({
      next: (data) => {
        this.details = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load mod details', err);
        this.isLoading = false;
      }
    });
  }

  public toggleFavourite(): void {
    if (!this.details) return;

    this.isFavouriteStatusChanging = true;

    const req = this.isFavourited
      ? this.modsService.removeModFromFavourites(this.details.id)
      : this.modsService.addModToFavourites(this.details.id);

    req.subscribe({
      next: () => {
        this.isFavourited = !this.isFavourited;
        this.details.favouriteCount += this.isFavourited ? 1 : -1;

        this.snackBar.open(
          this.isFavourited ? 'Added to favourites!' : 'Removed from favourites.',
          'Close',
          { duration: 3000 }
        );

        this.isFavouriteStatusChanging = false;
      },
      error: () => {
        this.snackBar.open('Error updating favourite status.', 'Close', { duration: 3000 });
        this.isFavouriteStatusChanging = false;
      }
    });
  }
}
