# Google OAuth Setup Guide

## Step 1: Create Google OAuth Credentials

1. Go to https://console.cloud.google.com
2. Create a new project (or select existing)
3. Go to **APIs & Services → Credentials**
4. Click **Create Credentials → OAuth 2.0 Client ID**
5. Choose **Web Application**
6. Add Authorized JavaScript Origins:
   - `http://localhost:4200`
7. Add Authorized Redirect URIs (not needed for frontend-flow, but add anyway):
   - `http://localhost:4200`
8. Copy the **Client ID** (looks like: `xxxxxxxx.apps.googleusercontent.com`)

## Step 2: Add Client ID to Backend

In `Weather.AuthService/appsettings.json`, replace:
```json
"Google": {
  "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
}
```
With your actual Client ID.

## Step 3: Run the New Migration

In Visual Studio Package Manager Console (select Weather.Infrastructure as default project):
```
Add-Migration AddGoogleAuthFields
Update-Database
```

Or via CLI:
```bash
dotnet ef migrations add AddGoogleAuthFields --project Weather.Infrastructure --startup-project Weather.AuthService
dotnet ef database update --project Weather.Infrastructure --startup-project Weather.AuthService
```

## Step 4: Frontend Integration (Angular)

Install the Google library:
```bash
npm install @abacritt/angularx-social-login
```

In your login component:
```typescript
import { SocialAuthService, GoogleLoginProvider } from '@abacritt/angularx-social-login';

// Trigger Google Sign-In
async loginWithGoogle() {
  const googleUser = await this.authService.signIn(GoogleLoginProvider.PROVIDER_ID);
  
  // Send idToken to your backend
  this.http.post('/api/auth/google-login', { idToken: googleUser.idToken })
    .subscribe((res: any) => {
      localStorage.setItem('token', res.token);
      // navigate to dashboard
    });
}
```

## API Endpoint

**POST** `/api/auth/google-login`

Request body:
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6..."
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6...",
  "expiresIn": 60,
  "user": {
    "id": 1,
    "email": "user@gmail.com",
    "fullName": "John Doe",
    "profilePicture": "https://lh3.googleusercontent.com/...",
    "role": "User",
    "loginProvider": "Google"
  }
}
```
