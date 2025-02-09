import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-login-page',
  templateUrl: 'login-page.component.html',
  styleUrls: ['login-page.component.scss'],  
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPageComponent {
  loginForm: FormGroup;
  errorMessage: string | null = null;

  constructor(private readonly fb: FormBuilder, private readonly authService: AuthService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe(
        (response) => {
          console.log('Login successful:', response);
          // Handle successful login (e.g., save token, navigate)
        },
        (error) => {
          this.errorMessage = 'Login failed. Please check your credentials.';
        }
      );
    }
  }
}
