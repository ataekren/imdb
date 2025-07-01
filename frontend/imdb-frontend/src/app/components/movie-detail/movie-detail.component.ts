import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { takeUntil, catchError } from 'rxjs/operators';

import { Movie } from '../../models/movie.model';
import { ApiService } from '../../services/api.service';
import { LanguageService } from '../../services/language.service';
import { AuthService } from '../../services/auth.service';
import { RatingPopupComponent } from '../rating-popup/rating-popup.component';
import { RatingRequest } from '../../models/auth.model';

@Component({
  selector: 'app-movie-detail',
  standalone: true,
  imports: [CommonModule, RatingPopupComponent],
  templateUrl: './movie-detail.component.html',
  styleUrls: ['./movie-detail.component.css']
})
export class MovieDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  movie: Movie | null = null;
  isLoading = true;
  error: string | null = null;
  isLoggedIn = false;
  
  // Rating popup properties
  showRatingPopup = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private languageService: LanguageService,
    private authService: AuthService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    // Subscribe to language changes
    this.languageService.currentLanguage$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        // Force template to update by creating a new reference
        if (this.movie) {
          this.movie = { ...this.movie };
        }
      });

    this.authService.isLoggedIn$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
      });

    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const movieId = +params['id'];
        if (movieId) {
          this.loadMovie(movieId);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadMovie(id: number): void {
    this.isLoading = true;
    this.error = null;

    this.apiService.getMovie(id)
      .pipe(
        takeUntil(this.destroy$),
        catchError(error => {
          this.error = this.translations.error;
          this.isLoading = false;
          console.error('Error loading movie:', error);
          return [];
        })
      )
      .subscribe(movie => {
        this.movie = movie;
        this.isLoading = false;
      });
  }

  onRateClick(): void {
    if (!this.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    this.showRatingPopup = true;
  }

  onRatingSubmit(data: { movieId: number, score: number }): void {
    const ratingRequest: RatingRequest = { score: data.score };
    
    this.apiService.rateMovie(data.movieId, ratingRequest).subscribe({
      next: (response: any) => {
        // Update the movie's user rating and movie data
        if (this.movie) {
          this.movie.userRating = data.score;
          
          // Update movie data with the recalculated values from backend
          if (response.updatedMovieData) {
            this.movie.imdbScore = response.updatedMovieData.imdbScore;
            this.movie.averageRating = response.updatedMovieData.averageRating;
            this.movie.totalRatings = response.updatedMovieData.totalRatings;
          }
        }
        this.closeRatingPopup();
      },
      error: (error) => {
        console.error('Error rating movie:', error);
        // TODO: Show error message to user
      }
    });
  }

  closeRatingPopup(): void {
    this.showRatingPopup = false;
  }

  onWatchlistToggle(): void {
    if (!this.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    if (!this.movie) return;

    if (this.movie.isInWatchlist) {
      this.apiService.removeFromWatchlist(this.movie.id).subscribe(() => {
        if (this.movie) {
          this.movie.isInWatchlist = false;
        }
      });
    } else {
      this.apiService.addToWatchlist(this.movie.id).subscribe(() => {
        if (this.movie) {
          this.movie.isInWatchlist = true;
        }
      });
    }
  }

  getMovieTitle(): string {
    if (!this.movie) return '';
    return this.languageService.getMovieTitle(this.movie);
  }

  getMovieSummary(): string {
    if (!this.movie) return '';
    return this.languageService.getMovieSummary(this.movie);
  }

  getYouTubeEmbedUrl(): SafeResourceUrl {
    if (!this.movie?.trailerUrl) {
      return this.sanitizer.bypassSecurityTrustResourceUrl('');
    }

    // Extract YouTube video ID from various YouTube URL formats
    let videoId = '';
    const url = this.movie.trailerUrl;
    
    // Handle different YouTube URL formats
    if (url.includes('youtube.com/watch?v=')) {
      videoId = url.split('v=')[1]?.split('&')[0] || '';
    } else if (url.includes('youtu.be/')) {
      videoId = url.split('youtu.be/')[1]?.split('?')[0] || '';
    } else if (url.includes('youtube.com/embed/')) {
      videoId = url.split('embed/')[1]?.split('?')[0] || '';
    }

    if (videoId) {
      const embedUrl = `https://www.youtube.com/embed/${videoId}?autoplay=0&rel=0&modestbranding=1`;
      return this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl);
    }

    return this.sanitizer.bypassSecurityTrustResourceUrl('');
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  goToRatingDetail(): void {
    if (this.movie) {
      this.router.navigate(['/movie', this.movie.id, 'ratings']);
    }
  }

  get translations() {
    return this.languageService.getTranslations();
  }
} 