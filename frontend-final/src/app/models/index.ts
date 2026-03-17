export interface RegisterRequest {
  email: string;
  password: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresIn: number;
}

export interface RegisterResponse {
  id: number;
  email: string;
  role: string;
}

export interface GoogleLoginRequest {
  idToken: string;
}

export interface GoogleAuthResponse {
  token: string;
  expiresIn: number;
  user: {
    id: number;
    email: string;
    fullName: string;
    profilePicture: string;
    role: string;
    loginProvider: string;
  };
}

export interface WeatherData {
  city: string;
  country: string;
  description: string;
  temperature: number;
  humidity: number;
  pressure: number;
  windSpeed: number;
  latitude: number;
  longitude: number;
  sunrise: number;
  sunset: number;
}
