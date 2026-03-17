import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { ThemeService } from '../../shared/services/theme.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  form: FormGroup;
  loading = false;
  googleLoading = false;
  error = '';
  success = '';
  showPassword = false;

  private googleClientId = '492031889007-jk7lbqff76mgevg7v2tp1gvkg0le3pqm.apps.googleusercontent.com';

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    public themeService: ThemeService
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: ['User', Validators.required]
    });
    if (this.auth.hasToken()) this.router.navigate(['/dashboard']);
  }

  ngOnInit(): void {
    this.auth.initGoogleSignIn(
      this.googleClientId,
      'google-btn-register',
      (idToken: string) => this.handleGoogleToken(idToken)
    );
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.error = '';
    this.auth.register(this.form.value).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Account created! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 1800);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error || 'Registration failed. Please try again.';
      }
    });
  }

  handleGoogleToken(idToken: string): void {
    this.googleLoading = true;
    this.error = '';
    this.auth.googleLogin(idToken).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.googleLoading = false;
        this.error = err.error || 'Google sign-in failed. Please try again.';
      }
    });
  }
}
