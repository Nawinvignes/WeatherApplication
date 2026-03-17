import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../shared/services/auth.service';
import { ThemeService } from '../shared/services/theme.service';

export interface WeatherLog {
  id: string;
  userId: string | null;
  city: string;
  requestedAt: string;
  errorMessage: string | null;
}

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class HistoryComponent implements OnInit {
  logs: WeatherLog[] = [];
  loading = true;
  error = '';

  private apiUrl = 'https://localhost:7179/api/WeatherLog';

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private router: Router,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void { this.loadLogs(); }

  loadLogs(): void {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    this.http.get<WeatherLog[]>(this.apiUrl, { headers }).subscribe({
      next: (data) => { this.logs = data; this.loading = false; },
      error: () => { this.error = 'Failed to load history. Make sure backend is running.'; this.loading = false; }
    });
  }

  goBack(): void { this.router.navigate(['/dashboard']); }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' });
  }

  formatTime(dateStr: string): string {
    return new Date(dateStr).toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  }

  get successCount(): number { return this.logs.filter(l => !l.errorMessage).length; }
  get errorCount(): number { return this.logs.filter(l => l.errorMessage).length; }
}
