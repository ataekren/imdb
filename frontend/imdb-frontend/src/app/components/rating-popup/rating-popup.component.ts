import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MovieSummary, Movie } from '../../models/movie.model';
import { LanguageService } from '../../services/language.service';

@Component({
  selector: 'app-rating-popup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rating-popup.component.html',
  styleUrls: ['./rating-popup.component.css']
})
export class RatingPopupComponent {
  @Input() movie: MovieSummary | Movie | null = null;
  @Input() isVisible: boolean = false;
  @Output() rateMovie = new EventEmitter<{ movieId: number, score: number }>();
  @Output() close = new EventEmitter<void>();

  selectedRating: number = 0;
  hoveredRating: number = 0;

  constructor(private languageService: LanguageService) {}

  onStarHover(rating: number): void {
    this.hoveredRating = rating;
  }

  onStarLeave(): void {
    this.hoveredRating = 0;
  }

  onStarClick(rating: number): void {
    this.selectedRating = rating;
  }

  onSubmitRating(): void {
    if (this.selectedRating > 0 && this.movie) {
      this.rateMovie.emit({
        movieId: this.movie.id,
        score: this.selectedRating
      });
      this.closePopup();
    }
  }

  closePopup(): void {
    this.selectedRating = 0;
    this.hoveredRating = 0;
    this.close.emit();
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closePopup();
    }
  }

  getStarClass(index: number): string {
    const rating = this.hoveredRating || this.selectedRating;
    return index <= rating ? 'star filled' : 'star';
  }

  getMovieTitle(): string {
    if (!this.movie) return '';
    return this.languageService.getMovieTitle(this.movie);
  }

  get translations() {
    return this.languageService.getTranslations();
  }
} 