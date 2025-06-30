import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { LanguageService } from '../../../services/language.service';
import { RegisterRequest } from '../../../models/auth.model';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  public languageService = inject(LanguageService);

  registerData: RegisterRequest = {
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    country: '',
    city: ''
  };

  confirmPassword = '';
  isLoading = false;
  error = '';

  countries = [
    'United States', 'Turkey', 'United Kingdom', 'Germany', 'France', 
    'Spain', 'Italy', 'Canada', 'Australia', 'Japan', 'Other'
  ];

  onSignup() {
    // Basic validation
    if (!this.registerData.email || !this.registerData.password || 
        !this.registerData.firstName || !this.registerData.lastName ||
        !this.registerData.country || !this.registerData.city) {
      this.error = this.languageService.translate('fillAllFields');
      return;
    }

    // Password validation
    if (this.registerData.password.length < 8) {
      this.error = 'Password must be at least 8 characters long';
      return;
    }

    if (!/(?=.*[0-9])/.test(this.registerData.password)) {
      this.error = 'Password must contain at least 1 number';
      return;
    }

    if (!/(?=.*[!@#$%^&*(),.?":{}|<>])/.test(this.registerData.password)) {
      this.error = 'Password must contain at least 1 special character';
      return;
    }

    if (this.registerData.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    this.isLoading = true;
    this.error = '';

    this.authService.register(this.registerData).subscribe({
      next: (response) => {
        console.log('Registration successful:', response);
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Registration failed:', error);
        this.error = 'Registration failed. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
