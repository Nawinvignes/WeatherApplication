import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WeatherData } from '../../models/index';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  private apiUrl = 'https://localhost:7179/api/weather';

  constructor(private http: HttpClient) {}

  getWeatherByCity(city: string): Observable<WeatherData> {
    return this.http.get<WeatherData>(`${this.apiUrl}/${city}`);
  }

  getWeatherByCoords(lat: number, lon: number): Observable<WeatherData> {
    return this.http.get<WeatherData>(`${this.apiUrl}/by-coordinates?lat=${lat}&lon=${lon}`);
  }
}
