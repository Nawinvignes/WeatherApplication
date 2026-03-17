import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, RegisterResponse, GoogleLoginRequest, GoogleAuthResponse } from '../../models/index';

declare const google: any;

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7285/api/Auth';
  private tokenKey = 'weather_token';
  private userKey = 'weather_user';
  private userProfileKey = 'weather_profile';

  isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(
    private http: HttpClient,
    private router: Router,
    private ngZone: NgZone
  ) {}

  // ==============================
  // NORMAL REGISTER
  // ==============================
  register(data: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, data);
  }

  // ==============================
  // NORMAL LOGIN
  // ==============================
  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.token);
        localStorage.setItem(this.userKey, data.email);
        this.isLoggedIn$.next(true);
      })
    );
  }

  // ==============================
  // GOOGLE LOGIN
  // ==============================
  googleLogin(idToken: string): Observable<GoogleAuthResponse> {
    const body: GoogleLoginRequest = { idToken };
    return this.http.post<GoogleAuthResponse>(`${this.apiUrl}/google-login`, body).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.token);
        localStorage.setItem(this.userKey, res.user.email);
        localStorage.setItem(this.userProfileKey, JSON.stringify(res.user));
        this.isLoggedIn$.next(true);
      })
    );
  }

  // ==============================
  // INITIALIZE GOOGLE SIGN-IN BUTTON
  // ==============================
  initGoogleSignIn(clientId: string, buttonElementId: string, onSuccess: (token: string) => void): void {
    const waitForGoogle = setInterval(() => {
      if (typeof google !== 'undefined' && google.accounts) {
        clearInterval(waitForGoogle);

        google.accounts.id.initialize({
          client_id: clientId,
          callback: (response: any) => {
            this.ngZone.run(() => {
              onSuccess(response.credential);
            });
          }
        });

        const btn = document.getElementById(buttonElementId);
        if (btn) {
          google.accounts.id.renderButton(btn, {
            theme: 'outline',
            size: 'large',
            width: '308',
            text: 'signin_with',
            shape: 'rectangular'
          });
        }
      }
    }, 100);
  }

  // ==============================
  // LOGOUT
  // ==============================
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    localStorage.removeItem(this.userProfileKey);
    this.isLoggedIn$.next(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null { return localStorage.getItem(this.tokenKey); }
  getUser(): string | null { return localStorage.getItem(this.userKey); }
  getUserProfile(): any {
    const p = localStorage.getItem(this.userProfileKey);
    return p ? JSON.parse(p) : null;
  }
  hasToken(): boolean { return !!localStorage.getItem(this.tokenKey); }
}
