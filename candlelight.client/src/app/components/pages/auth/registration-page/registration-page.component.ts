import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-registration-page',
  templateUrl: 'registration-page.component.html',
  styleUrls: ['registration-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegistrationPageComponent {
  registrationForm: FormGroup;
  errorMessage: string | null = null;

  constructor(private readonly fb: FormBuilder, private readonly authService: AuthService) {
    this.registrationForm = this.fb.group({
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.registrationForm.valid) {
      this.authService.register(this.registrationForm.value).subscribe(
        (response) => {
          console.log('Registration successful:', response);
          // Handle successful registration (e.g., navigate to login)
        },
        (error) => {
          this.errorMessage = 'Registration failed. Please try again.';
        }
      );
    }
  }
}
