import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { ApiService } from '../../services/api.service';
import { LanguageService } from '../../services/language.service';
import { RatingDistribution, Movie } from '../../models/movie.model';

interface BarData {
  score: number;
  count: number;
  percent: number;
}

@Component({
  selector: 'app-rating-detail-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rating-detail-page.component.html',
  styleUrls: ['./rating-detail-page.component.css']
})
export class RatingDetailPageComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  movieId!: number;
  distributions: RatingDistribution[] = [];
  countries: string[] = [];
  selectedCountry: string = 'All';
  bars: BarData[] = [];
  totalRatings = 0;
  isLoading = true;
  error = '';
  movie: Movie | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private language: LanguageService
  ) {}

  ngOnInit(): void {
    this.movieId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.movieId) {
      this.router.navigate(['/']);
      return;
    }

    forkJoin({
      movie: this.apiService.getMovie(this.movieId),
      dist: this.apiService.getRatingDistribution(this.movieId)
    })
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: res => {
        this.movie = res.movie;
        this.distributions = res.dist;
        this.prepareCountries();
        this.selectCountry('All');
        this.isLoading = false;
      },
      error: err => {
        console.error(err);
        this.error = this.language.translate('error');
        this.isLoading = false;
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  prepareCountries(): void {
    const sorted = [...this.distributions].sort((a,b)=> b.totalRatings - a.totalRatings);
    this.countries = sorted.map(d=>d.country).slice(0,5);
  }

  selectCountry(country: string): void {
    this.selectedCountry = country;
    let counts: { [score:number]: number } = {};
    if (country === 'All') {
      for (const dist of this.distributions) {
        for (const key in dist.ratingCounts) {
          const score = Number(key);
          counts[score] = (counts[score] || 0) + dist.ratingCounts[key];
        }
      }
      this.totalRatings = Object.values(counts).reduce((a,b)=>a+b,0);
    } else {
      const dist = this.distributions.find(d=>d.country===country);
      if (dist) {
        counts = dist.ratingCounts;
        this.totalRatings = dist.totalRatings;
      }
    }

    this.bars = [];
    for (let score = 10; score >=1; score--) {
      const count = counts[score] || 0;
      const percent = this.totalRatings? (count/this.totalRatings)*100 : 0;
      this.bars.push({score, count, percent});
    }
  }

  get translations() {
    return this.language.getTranslations();
  }

  formatPercent(value: number): string {
    return value.toFixed(1);
  }

  formatCount(count: number): string {
    if (count >= 1000) {
      return (count/1000).toFixed(1)+'K';
    }
    return count.toString();
  }
} 