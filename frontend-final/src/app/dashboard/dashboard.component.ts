import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { WeatherService } from '../shared/services/weather.service';
import { AuthService } from '../shared/services/auth.service';
import { ThemeService } from '../shared/services/theme.service';
import { WeatherData } from '../models/index';
import * as L from 'leaflet';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  weatherData: WeatherData | null = null;
  loading = false;
  error = '';
  cityInput = '';
  userName = '';
  userProfilePic = '';
  isGoogleUser = false;
  private map!: L.Map;
  private marker!: L.Marker;
  private timeInterval!: ReturnType<typeof setInterval>;
  recentSearches: string[] = [];

  weatherCards = [
    { key: 'humidity', label: 'Humidity', icon: '💧', unit: '%', color: 'var(--accent)' },
    { key: 'pressure', label: 'Pressure', icon: '🌡️', unit: ' hPa', color: '#9b59b6' },
    { key: 'windSpeed', label: 'Wind Speed', icon: '💨', unit: ' m/s', color: 'var(--success)' },
  ];

  constructor(
    private weather: WeatherService,
    public auth: AuthService,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void {
    const profile = this.auth.getUserProfile();
    if (profile && profile.loginProvider === 'Google') {
      this.userName = profile.fullName || profile.email.split('@')[0];
      this.userProfilePic = profile.profilePicture || '';
      this.isGoogleUser = true;
    } else {
      const email = this.auth.getUser() || '';
      this.userName = email.split('@')[0] || 'Explorer';
    }

    this.timeInterval = setInterval(() => {}, 60000);
    const saved = localStorage.getItem('recentSearches');
    if (saved) this.recentSearches = JSON.parse(saved);
  }

  ngAfterViewInit(): void { this.initMap(); }
  ngOnDestroy(): void {
    clearInterval(this.timeInterval);
    if (this.map) this.map.remove();
  }

  initMap(): void {
    this.map = L.map('map', { center: [20.5937, 78.9629], zoom: 4, zoomControl: false });
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '©<a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
      maxZoom: 19
    }).addTo(this.map);
    L.control.zoom({ position: 'bottomright' }).addTo(this.map);
    this.map.on('click', (e: L.LeafletMouseEvent) => {
      const { lat, lng } = e.latlng;
      this.fetchByCoords(lat, lng);
    });
  }

  fetchByCity(): void {
    if (!this.cityInput.trim()) return;
    this.loading = true; this.error = '';
    this.weather.getWeatherByCity(this.cityInput.trim()).subscribe({
      next: (data) => {
        this.weatherData = data; this.loading = false;
        this.addToRecent(this.cityInput.trim());
        this.flyToLocation(data.latitude, data.longitude);
      },
      error: (err) => { this.loading = false; this.error = err.error?.message || 'City not found.'; }
    });
  }

  fetchByCoords(lat: number, lng: number): void {
    this.loading = true; this.error = '';
    this.weather.getWeatherByCoords(lat, lng).subscribe({
      next: (data) => {
        this.weatherData = data; this.loading = false;
        this.cityInput = data.city;
        this.addToRecent(data.city);
        this.updateMapMarker(lat, lng, data);
      },
      error: (err) => { this.loading = false; this.error = err.error?.message || 'Could not fetch weather.'; }
    });
  }

  flyToLocation(lat: number, lng: number): void {
    if (!this.map) return;
    this.map.flyTo([lat, lng], 10, { animate: true, duration: 1.5 });
    this.updateMapMarker(lat, lng, this.weatherData!);
  }

  updateMapMarker(lat: number, lng: number, data: WeatherData): void {
    if (this.marker) this.map.removeLayer(this.marker);
    const icon = L.divIcon({
      className: 'custom-marker',
      html: `<div class="marker-inner"><div class="marker-dot"></div><div class="marker-label">${data.temperature.toFixed(1)}°C</div></div>`,
      iconSize: [80, 40], iconAnchor: [40, 20]
    });
    this.marker = L.marker([lat, lng], { icon }).addTo(this.map);
    L.popup({ className: 'custom-popup' })
      .setContent(`<b>${data.city}, ${data.country}</b><br>${data.description} · ${data.temperature.toFixed(1)}°C`)
      .openOn(this.map);
  }

  addToRecent(city: string): void {
    this.recentSearches = [city, ...this.recentSearches.filter(c => c !== city)].slice(0, 5);
    localStorage.setItem('recentSearches', JSON.stringify(this.recentSearches));
  }

  getWeatherIcon(desc: string): string {
    const d = desc.toLowerCase();
    if (d.includes('clear') || d.includes('sunny')) return '☀️';
    if (d.includes('cloud')) return '⛅';
    if (d.includes('rain') || d.includes('drizzle')) return '🌧️';
    if (d.includes('thunder')) return '⛈️';
    if (d.includes('snow')) return '❄️';
    if (d.includes('mist') || d.includes('fog') || d.includes('haze')) return '🌫️';
    return '🌤️';
  }

  formatTime(unix: number): string {
    return new Date(unix * 1000).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  getCardValue(key: string): number {
    if (!this.weatherData) return 0;
    return (this.weatherData as any)[key] ?? 0;
  }
}
