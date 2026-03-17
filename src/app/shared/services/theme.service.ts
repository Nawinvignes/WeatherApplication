import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private themeKey = 'weather_theme';
  private _dark = new BehaviorSubject<boolean>(this.loadPreference());
  isDark$ = this._dark.asObservable();

  constructor() {
    this.applyTheme(this._dark.value);
  }

  get isDark(): boolean { return this._dark.value; }

  toggle(): void {
    const next = !this._dark.value;
    this._dark.next(next);
    localStorage.setItem(this.themeKey, next ? 'dark' : 'light');
    this.applyTheme(next);
  }

  private loadPreference(): boolean {
    const saved = localStorage.getItem(this.themeKey);
    if (saved) return saved === 'dark';
    return window.matchMedia?.('(prefers-color-scheme: dark)').matches ?? false;
  }

  private applyTheme(dark: boolean): void {
    document.documentElement.setAttribute('data-theme', dark ? 'dark' : 'light');
  }
}
