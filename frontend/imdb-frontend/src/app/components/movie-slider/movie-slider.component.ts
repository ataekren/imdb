import { Component, OnInit, OnDestroy, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, throttleTime } from 'rxjs/operators';

import { MovieSummary } from '../../models/movie.model';
import { LanguageService } from '../../services/language.service';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';
import { RatingPopupComponent } from '../rating-popup/rating-popup.component';
import { RatingRequest } from '../../models/auth.model';

@Component({
  selector: 'app-movie-slider',
  standalone: true,
  imports: [CommonModule, RatingPopupComponent],
  templateUrl: './movie-slider.component.html',
  styleUrls: ['./movie-slider.component.css']
})
export class MovieSliderComponent implements OnInit, OnDestroy, OnChanges {
  @Input() movies: MovieSummary[] = [];
  @Input() title: string = '';
  
  private destroy$ = new Subject<void>();
  private resize$ = new Subject<void>();
  
  currentSlide = 0;
  slidesToShow = 5;
  isLoggedIn = false;
  
  // Rating popup properties
  showRatingPopup = false;
  movieToRate: MovieSummary | null = null;

  constructor(
    private router: Router,
    private languageService: LanguageService,
    private authService: AuthService,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.authService.isLoggedIn$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
      });

    this.updateSlidesToShow();
    
    // Throttle resize events for better performance
    this.resize$
      .pipe(
        takeUntil(this.destroy$),
        throttleTime(100)
      )
      .subscribe(() => {
        this.updateSlidesToShow();
      });

    window.addEventListener('resize', this.onResize.bind(this));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    window.removeEventListener('resize', this.onResize.bind(this));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['movies'] && this.movies) {
      // Ensure we only keep the first 10 movies
      if (this.movies.length > 10) {
        this.movies = this.movies.slice(0, 10);
      }
    }
  }

  private onResize(): void {
    this.resize$.next();
  }

  private updateSlidesToShow(): void {
    const width = window.innerWidth;
    if (width < 480) {
      this.slidesToShow = 2;
    } else if (width < 768) {
      this.slidesToShow = 3;
    } else if (width < 1024) {
      this.slidesToShow = 4;
    } else {
      this.slidesToShow = 5;
    }
  }

  get maxSlide(): number {
    return Math.max(0, this.movies.length - this.slidesToShow);
  }

  previousSlide(): void {
    this.currentSlide = Math.max(0, this.currentSlide - 1);
  }

  nextSlide(): void {
    this.currentSlide = Math.min(this.maxSlide, this.currentSlide + 1);
  }

  getSliderTransform(): string {
    const translateX = -(this.currentSlide * (100 / this.slidesToShow));
    return `translateX(${translateX}%)`;
  }

  onMovieClick(movie: MovieSummary): void {
    this.router.navigate(['/movie', movie.id]);
  }

  onWatchlistToggle(event: Event, movie: MovieSummary): void {
    event.stopPropagation();
    
    if (!this.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    // TODO: Implement watchlist toggle
    if (movie.isInWatchlist) {
      this.apiService.removeFromWatchlist(movie.id).subscribe(() => {
        movie.isInWatchlist = false;
      });
    } else {
      this.apiService.addToWatchlist(movie.id).subscribe(() => {
        movie.isInWatchlist = true;
      });
    }
  }

  onTrailerClick(event: Event, movie: MovieSummary): void {
    event.stopPropagation();
    // Navigate to movie detail page instead of opening trailer directly
    this.router.navigate(['/movie', movie.id]);
  }

  onRateClick(event: Event, movie: MovieSummary): void {
    event.stopPropagation();
    
    if (!this.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    this.movieToRate = movie;
    this.showRatingPopup = true;
  }

  onRatingSubmit(data: { movieId: number, score: number }): void {
    const ratingRequest: RatingRequest = { score: data.score };
    
    this.apiService.rateMovie(data.movieId, ratingRequest).subscribe({
      next: (response: any) => {
        // Update the movie's user rating and movie data
        if (this.movieToRate) {
          this.movieToRate.userRating = data.score;
          
          // Update movie data with the recalculated values from backend
          if (response.updatedMovieData) {
            this.movieToRate.imdbScore = response.updatedMovieData.imdbScore;
            this.movieToRate.averageRating = response.updatedMovieData.averageRating;
            this.movieToRate.totalRatings = response.updatedMovieData.totalRatings;
          }
          
          // Update the movie in the movies array
          const movieIndex = this.movies.findIndex(m => m.id === data.movieId);
          if (movieIndex !== -1) {
            this.movies[movieIndex].userRating = data.score;
            
            // Update movie data in the array as well
            if (response.updatedMovieData) {
              this.movies[movieIndex].imdbScore = response.updatedMovieData.imdbScore;
              this.movies[movieIndex].averageRating = response.updatedMovieData.averageRating;
              this.movies[movieIndex].totalRatings = response.updatedMovieData.totalRatings;
            }
          }
        }
        this.closeRatingPopup();
        
        // Refresh the page to show updated data
        window.location.reload();
      },
      error: (error) => {
        console.error('Error rating movie:', error);
        // TODO: Show error message to user
      }
    });
  }

  closeRatingPopup(): void {
    this.showRatingPopup = false;
    this.movieToRate = null;
  }

  getMovieTitle(movie: MovieSummary): string {
    return this.languageService.getMovieTitle(movie);
  }

  get translations() {
    return this.languageService.getTranslations();
  }

  onRatingDetail(event: Event, movie: MovieSummary): void {
    event.stopPropagation();
    this.router.navigate(['/movie', movie.id, 'ratings']);
  }
} 