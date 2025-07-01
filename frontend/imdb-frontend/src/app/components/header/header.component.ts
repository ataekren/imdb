import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, Observable, of } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';

import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';
import { LanguageService, Language } from '../../services/language.service';
import { User } from '../../models/auth.model';
import { MovieSummary, Actor, SearchResult } from '../../models/movie.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  currentUser: User | null = null;
  isLoggedIn = false;
  currentLanguage: Language = 'en';
  
  searchControl = new FormControl('');
  searchTypeControl = new FormControl('all');
  
  searchResults$: Observable<SearchResult> = of({ movies: [], actors: [] });
  isSearching = false;
  showSearchResults = false;

  // Profile picture upload properties
  showProfileUpload = false;
  selectedFile: File | null = null;
  profilePicturePreview: string | null = null;
  isUploading = false;
  uploadError = '';
  uploadSuccess = '';

  constructor(
    private authService: AuthService,
    private apiService: ApiService,
    private languageService: LanguageService,
    public router: Router
  ) {}

  ngOnInit(): void {
    // Subscribe to authentication state
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
        this.isLoggedIn = !!user;
      });

    // Subscribe to language changes
    this.languageService.currentLanguage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(language => {
        this.currentLanguage = language;
      });

    // Setup search functionality
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearch(): void {
    this.searchResults$ = this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        if (!query || query.length < 3) {
          this.showSearchResults = false;
          return of({ movies: [], actors: [] });
        }
        
        this.isSearching = true;
        this.showSearchResults = true;
        
        const searchType = this.searchTypeControl.value || undefined;
        
        return this.apiService.searchMovies(query, searchType, 3).pipe(
          catchError(() => of({ movies: [], actors: [] })),
          switchMap(results => {
            this.isSearching = false;
            return of(results);
          })
        );
      })
    );
  }

  onLogin(): void {
    this.router.navigate(['/login']);
  }

  onSignup(): void {
    this.router.navigate(['/signup']);
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  toggleProfileUpload(): void {
    this.showProfileUpload = !this.showProfileUpload;
    if (!this.showProfileUpload) {
      this.resetUploadState();
    }
  }

  closeProfileUpload(event: Event): void {
    if (this.showProfileUpload) {
      this.showProfileUpload = false;
      this.resetUploadState();
    }
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
      if (!allowedTypes.includes(file.type)) {
        this.uploadError = 'Please select a valid image file (JPEG, PNG, GIF, or WebP)';
        return;
      }

      // Validate file size (5MB max)
      if (file.size > 5 * 1024 * 1024) {
        this.uploadError = 'File size must be less than 5MB';
        return;
      }

      this.selectedFile = file;
      this.uploadError = '';
      this.uploadSuccess = '';

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.profilePicturePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  uploadProfilePicture(): void {
    if (!this.selectedFile) {
      this.uploadError = 'Please select a file first';
      return;
    }

    this.isUploading = true;
    this.uploadError = '';
    this.uploadSuccess = '';

    this.authService.uploadProfilePicture(this.selectedFile).subscribe({
      next: (response) => {
        console.log('Profile picture uploaded successfully:', response);
        this.uploadSuccess = 'Profile picture updated successfully!';
        this.isUploading = false;
        this.resetUploadState();
        
        // Auto-close dropdown after success
        setTimeout(() => {
          this.showProfileUpload = false;
        }, 2000);
      },
      error: (error) => {
        console.error('Profile picture upload failed:', error);
        this.uploadError = 'Failed to upload profile picture. Please try again.';
        this.isUploading = false;
      }
    });
  }

  removeProfilePicture(): void {
    this.selectedFile = null;
    this.profilePicturePreview = null;
    this.uploadError = '';
    this.uploadSuccess = '';
    
    // Reset file input
    const fileInput = document.getElementById('headerProfilePicture') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  private resetUploadState(): void {
    this.selectedFile = null;
    this.profilePicturePreview = null;
    this.uploadError = '';
    this.uploadSuccess = '';
    
    // Reset file input
    const fileInput = document.getElementById('headerProfilePicture') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  onLanguageChange(language: Language): void {
    this.languageService.setLanguage(language);
  }

  onSearch(): void {
    const query = this.searchControl.value;
    const searchType = this.searchTypeControl.value;
    
    if (query && query.length >= 3) {
      this.router.navigate(['/search'], {
        queryParams: {
          q: query,
          type: searchType !== 'all' ? searchType : undefined
        }
      });
      this.hideSearchResults();
    }
  }

  onMovieSelect(movie: MovieSummary): void {
    this.router.navigate(['/movie', movie.id]);
    this.hideSearchResults();
    this.searchControl.setValue('');
  }

  onActorSelect(actor: Actor): void {
    // For now, just log the actor selection since we don't have actor details page yet
    console.log('Selected actor:', actor);
    this.hideSearchResults();
    this.searchControl.setValue('');
  }

  hideSearchResults(): void {
    this.showSearchResults = false;
  }

  onSearchBlur(): void {
    // Delay hiding to allow click events on search results
    setTimeout(() => {
      this.hideSearchResults();
    }, 200);
  }

  get translations() {
    return this.languageService.getTranslations();
  }

  getMovieTitle(movie: MovieSummary): string {
    return this.languageService.getMovieTitle(movie);
  }

  getInitials(user: User | null): string {
    if (!user) return '';
    const firstInitial = user.firstName ? user.firstName.charAt(0).toUpperCase() : '';
    const lastInitial = user.lastName ? user.lastName.charAt(0).toUpperCase() : '';
    return firstInitial + lastInitial;
  }
} 