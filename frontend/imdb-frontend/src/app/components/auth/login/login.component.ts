import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Auth, GoogleAuthProvider, signInWithPopup } from '@angular/fire/auth';
import { AuthService } from '../../../services/auth.service';
import { LanguageService } from '../../../services/language.service';
import { LoginRequest, GoogleAuthRequest } from '../../../models/auth.model';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private auth = inject(Auth);
  private authService = inject(AuthService);
  private router = inject(Router);
  public languageService = inject(LanguageService);

  loginData: LoginRequest = {
    email: '',
    password: ''
  };

  isLoading = false;
  error = '';

  async onGoogleLogin() {
    this.isLoading = true;
    this.error = '';

    try {
      const provider = new GoogleAuthProvider();
      const result = await signInWithPopup(this.auth, provider);
      const idToken = await result.user.getIdToken();

      // Extract display name from Google user
      const displayName = result.user.displayName || '';
      const nameParts = displayName.trim().split(' ');
      
      // Split name into first and last name
      const firstName = nameParts[0] || '';
      const lastName = nameParts.length > 1 ? nameParts.slice(1).join(' ') : '';

      const googleAuthRequest: GoogleAuthRequest = {
        idToken: idToken,
        firstName: firstName,
        lastName: lastName,
        country: 'Turkey',      // Default country
        city: 'Istanbul'        // Default city
      };

      this.authService.googleAuth(googleAuthRequest).subscribe({
        next: (response) => {
          console.log('Google login successful:', response);
          this.router.navigate(['/']);
        },
        error: (error) => {
          console.error('Google login failed:', error);
          this.error = this.languageService.translate('googleLoginFailed');
          this.isLoading = false;
        }
      });
    } catch (error) {
      console.error('Google sign-in failed:', error);
      this.error = this.languageService.translate('googleSignInFailed');
      this.isLoading = false;
    }
  }

  onLogin() {
    if (!this.loginData.email || !this.loginData.password) {
      this.error = this.languageService.translate('fillAllFields');
      return;
    }

    this.isLoading = true;
    this.error = '';

    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        console.log('Login successful:', response);
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Login failed:', error);
        this.error = this.languageService.translate('loginFailed');
        this.isLoading = false;
      }
    });
  }
}
