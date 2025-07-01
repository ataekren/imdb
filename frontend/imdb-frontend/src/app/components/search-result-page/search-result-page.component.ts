import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { LanguageService } from '../../services/language.service';
import { MovieSummary, Actor, SearchResult } from '../../models/movie.model';
import { Subject, switchMap, takeUntil, of } from 'rxjs';

@Component({
  selector: 'app-search-result-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './search-result-page.component.html',
  styleUrls: ['./search-result-page.component.css']
})
export class SearchResultPageComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  query = '';
  searchType: string | undefined;
  results: SearchResult = { movies: [], actors: [] };
  isLoading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private languageService: LanguageService
  ) {}

  ngOnInit(): void {
    // Listen for query param changes and fetch results
    this.route.queryParamMap
      .pipe(
        takeUntil(this.destroy$),
        switchMap(params => {
          this.query = params.get('q') || '';
          // If no query, redirect to home
          if (!this.query || this.query.length < 3) {
            this.router.navigate(['/']);
            return of(null);
          }
          this.searchType = params.get('type') || undefined;
          this.isLoading = true;
          this.error = '';
          return this.apiService.searchMovies(this.query, this.searchType, 20);
        })
      )
      .subscribe({
        next: res => {
          if (res) {
            this.results = res;
          }
          this.isLoading = false;
        },
        error: err => {
          console.error('Search failed', err);
          this.error = this.languageService.translate('error');
          this.isLoading = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onMovieSelect(movie: MovieSummary): void {
    this.router.navigate(['/movie', movie.id]);
  }

  onActorSelect(actor: Actor): void {
    // For future implementation, currently just log
    console.log('Actor selected', actor);
  }

  get translations() {
    return this.languageService.getTranslations();
  }

  getMovieTitle(movie: MovieSummary): string {
    return this.languageService.getMovieTitle(movie);
  }
} 