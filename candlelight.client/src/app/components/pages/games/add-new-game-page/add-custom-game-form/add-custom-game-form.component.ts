import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { GameService } from '../../services/game.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-custom-game-form',
  templateUrl: './add-custom-game-form.component.html',
  styleUrl: './add-custom-game-form.component.scss'
})
export class AddCustomGameFormComponent {
  form: FormGroup;
  coverFile?: File;
  selectedFileName = 'Choose a file...';

  constructor(
    private fb: FormBuilder, 
    private gameService: GameService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: [''],
      description: [''],
      developer: [''],
      publisher: [''],
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFileName = input.files[0].name;
      this.coverFile = input.files[0];
    }
  }

  submit(): void {
    const formData = new FormData();
    formData.append('name', this.form.value.name);
    formData.append('description', this.form.value.description);
    if (this.coverFile) {
      formData.append('coverImage', this.coverFile);
    }
    this.gameService.uploadCustomGame(formData)
    .subscribe(
    {
      next: (game) => {
        this.snackBar.open('🎉 Game added successfully!', 'Close', {
          duration: 3000,
          panelClass: 'snackbar-success'
        });
              setTimeout(() => {
        this.router.navigate(['/games', game]);
      }, 1000);
      },
      error: (err) => {
        this.snackBar.open(`❌ Upload failed: ${err.error?.message ?? 'Unknown error'}`, 'Close', {
          duration: 5000,
          panelClass: 'snackbar-error'
        });
      }
    });
  }
}
