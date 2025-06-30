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
import { MovieSummary } from '../../models/movie.model';

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
  
  searchResults$: Observable<MovieSummary[]> = of([]);
  isSearching = false;
  showSearchResults = false;

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
          return of([]);
        }
        
        this.isSearching = true;
        this.showSearchResults = true;
        
        return this.apiService.quickSearch(query, 3).pipe(
          catchError(() => of([])),
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
} 