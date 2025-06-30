import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil, catchError } from 'rxjs/operators';

import { MovieSliderComponent } from '../movie-slider/movie-slider.component';
import { ApiService } from '../../services/api.service';
import { LanguageService } from '../../services/language.service';
import { MovieSummary } from '../../models/movie.model';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    CommonModule,
    MovieSliderComponent
  ],
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css']
})
export class HomepageComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  popularMovies: MovieSummary[] = [];
  isLoading = true;
  error: string | null = null;

  constructor(
    private apiService: ApiService,
    private languageService: LanguageService
  ) {}

  ngOnInit(): void {
    this.loadPopularMovies();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadPopularMovies(): void {
    this.isLoading = true;
    this.error = null;

    this.apiService.getPopularMovies(15)
      .pipe(
        takeUntil(this.destroy$),
        catchError(error => {
          this.error = this.translations.error;
          this.isLoading = false;
          console.error('Error loading popular movies:', error);
          return [];
        })
      )
      .subscribe(movies => {
        this.popularMovies = movies;
        this.isLoading = false;
      });
  }

  get translations() {
    return this.languageService.getTranslations();
  }
} 