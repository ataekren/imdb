import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, throttleTime } from 'rxjs/operators';

import { MovieSummary } from '../../models/movie.model';
import { LanguageService } from '../../services/language.service';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-movie-slider',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-slider.component.html',
  styleUrls: ['./movie-slider.component.css']
})
export class MovieSliderComponent implements OnInit, OnDestroy {
  @Input() movies: MovieSummary[] = [];
  @Input() title: string = '';
  
  private destroy$ = new Subject<void>();
  private resize$ = new Subject<void>();
  
  currentSlide = 0;
  slidesToShow = 5;
  isLoggedIn = false;

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
    
    if (movie.trailerUrl) {
      window.open(movie.trailerUrl, '_blank');
    }
  }

  getMovieTitle(movie: MovieSummary): string {
    return this.languageService.getMovieTitle(movie);
  }

  get translations() {
    return this.languageService.getTranslations();
  }
} 