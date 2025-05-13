import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { matchStrings } from '../../../../shared/validators/match-strings.validator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration-page',
  templateUrl: 'registration-page.component.html',
  styleUrls: ['registration-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegistrationPageComponent {
  registrationForm: FormGroup;
  errorMessage: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
  ) {
    this.registrationForm = this.fb.group(
      {
        username: ['', [Validators.required]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
      },
      {
        validators: matchStrings('password', 'confirmPassword'),
      },
    );
  }

  onSubmit(): void {
    if (this.registrationForm.invalid) {
      return;
    }

    this.authService.register(this.registrationForm.value).subscribe({
      next: () => {
        this.snackBar.open(
          'Registration successful! You can now login.',
          'OK',
          {
            duration: 3000,
            panelClass: ['snackbar-success'],
          },
        );
        this.router.navigate(['/login']);
      },
      error: (err: string) => {
        this.snackBar.open(
          'Registration failed. Please try again. Error: ' + err,
          'Close',
          {
            duration: 3000,
            panelClass: ['snackbar-error'],
          },
        );
        this.router.navigate(['/register']);
      },
    });
  }
}
