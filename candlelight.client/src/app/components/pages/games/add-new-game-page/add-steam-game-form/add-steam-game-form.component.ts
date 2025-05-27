import { Component } from '@angular/core';
import { SteamService } from '../../services/steam.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-steam-game-form',
  templateUrl: './add-steam-game-form.component.html',
  styleUrl: './add-steam-game-form.component.scss'
})
export class AddSteamGameFormComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private steamService: SteamService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    this.form = this.fb.group({
      appId: [null, [Validators.required, Validators.min(1), Validators.pattern(/^\d+$/)]]
    });
  }
  
  submit(): void {
    this.steamService.getGameByAppId(this.form.value.appId)
    .subscribe(
      {
    next: (game) => {
      this.snackBar.open('✅ Game fetched successfully!', 'Go', {
        duration: 4000,
        panelClass: 'snackbar-success'
      });
      setTimeout(() => {
        this.router.navigate(['/games', game.appId]);
      }, 1000);
      },
      error: (err) => {
        const msg = err.status === 404
          ? `Game with App ID ${this.form.value.appId} not found.`
          : 'Something went wrong while fetching the game.';
        this.snackBar.open(`❌ ${msg}`, 'Close', {
          duration: 5000,
          panelClass: 'snackbar-error'
        });
      }
      }
    );
  }
}
