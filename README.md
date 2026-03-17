# SkyCast — Angular Weather Dashboard

A beautiful dark-themed weather dashboard connected to your backend APIs.

## 🚀 Quick Start

### Prerequisites
- Node.js 18+
- Angular CLI: `npm install -g @angular/cli`
- Your backend running on ports **7285** (Auth) and **7179** (Weather)

### Installation

```bash
# 1. Extract the zip and navigate in
cd weather-app

# 2. Install dependencies
npm install

# 3. Start the dev server
ng serve

# 4. Open your browser
http://localhost:4200
```

## 🔗 API Endpoints Used

| Method | URL | Description |
|--------|-----|-------------|
| POST | `http://localhost:7285/api/Auth/register` | Register new user |
| POST | `http://localhost:7285/api/Auth/login` | Login (returns JWT) |
| GET | `http://localhost:7179/api/weather/{city}` | Weather by city |
| GET | `http://localhost:7179/api/weather/by-coordinates?lat=&lon=` | Weather by coords |

## ✨ Features

- **Auth** — Login & Register with JWT token (stored in localStorage)
- **Auth Guard** — Protects dashboard route
- **JWT Interceptor** — Auto-attaches Bearer token to weather requests
- **City Search** — Search weather by city name
- **Map Click** — Click anywhere on the interactive dark map to fetch weather
- **Recent Searches** — Last 5 cities remembered across sessions
- **Animated UI** — Smooth animations, pulse markers, gradient cards
- **Dark Theme** — Deep space aesthetic with glass morphism

## 📁 Project Structure

```
src/app/
├── auth/
│   ├── login/          # Login page
│   └── register/       # Register page
├── dashboard/          # Main weather dashboard
├── shared/
│   ├── services/       # AuthService, WeatherService
│   ├── guards/         # authGuard
│   └── interceptors/   # jwtInterceptor
├── models/             # TypeScript interfaces
├── app.routes.ts       # Routing config
└── app.config.ts       # App providers
```

## 🌐 CORS Note

If you get CORS errors, add this to your .NET backend `Program.cs`:

```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
app.UseCors("AllowAngular");
```
