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

  // Profile picture properties
  selectedFile: File | null = null;
  profilePicturePreview: string | null = null;
  isUploadingProfilePicture = false;

  countries = [
    'United States', 'Turkey', 'United Kingdom', 'Germany', 'France', 
    'Spain', 'Italy', 'Canada', 'Australia', 'Japan', 'Other'
  ];

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
      if (!allowedTypes.includes(file.type)) {
        this.error = 'Please select a valid image file (JPEG, PNG, GIF, or WebP)';
        return;
      }

      // Validate file size (5MB max)
      if (file.size > 5 * 1024 * 1024) {
        this.error = 'File size must be less than 5MB';
        return;
      }

      this.selectedFile = file;
      this.error = '';

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.profilePicturePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  removeProfilePicture() {
    this.selectedFile = null;
    this.profilePicturePreview = null;
  }

  private uploadProfilePicture() {
    if (!this.selectedFile) {
      this.router.navigate(['/']);
      return;
    }

    this.isUploadingProfilePicture = true;
    
    this.authService.uploadProfilePicture(this.selectedFile).subscribe({
      next: (response) => {
        console.log('Profile picture uploaded successfully:', response);
        this.isUploadingProfilePicture = false;
        this.isLoading = false;
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Profile picture upload failed:', error);
        this.isUploadingProfilePicture = false;
        this.isLoading = false;
        // Still navigate to home even if profile picture upload fails
        // The user is already registered successfully
        this.router.navigate(['/']);
      }
    });
  }

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
        
        // If a profile picture was selected, upload it after successful registration
        if (this.selectedFile) {
          this.uploadProfilePicture();
        } else {
        this.router.navigate(['/']);
        }
      },
      error: (error) => {
        console.error('Registration failed:', error);
        this.error = 'Registration failed. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
