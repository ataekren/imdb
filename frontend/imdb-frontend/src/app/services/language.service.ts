import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

export type Language = 'en' | 'tr';

export interface Translations {
  // Header
  login: string;
  logout: string;
  register: string;
  welcome: string;
  profile: string;
  
  // Search
  searchPlaceholder: string;
  searchMovies: string;
  searchActors: string;
  searchAll: string;
  
  // Movies
  popularMovies: string;
  imdbScore: string;
  year: string;
  duration: string;
  director: string;
  genre: string;
  addToWatchlist: string;
  removeFromWatchlist: string;
  rate: string;
  comment: string;
  watchTrailer: string;
  
  // Common
  loading: string;
  error: string;
  noResults: string;
  minutes: string;
  
  // Movie details
  cast: string;
  summary: string;
  popularity: string;
  ranking: string;
  comments: string;
  ratings: string;
}

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private currentLanguageSubject = new BehaviorSubject<Language>('en');
  public currentLanguage$ = this.currentLanguageSubject.asObservable();

  private translations: Record<Language, Translations> = {
    en: {
      // Header
      login: 'Login',
      logout: 'Logout',
      register: 'Register',
      welcome: 'Welcome',
      profile: 'Profile',
      
      // Search
      searchPlaceholder: 'Search movies, actors...',
      searchMovies: 'Movies',
      searchActors: 'Actors',
      searchAll: 'All',
      
      // Movies
      popularMovies: 'Popular Movies',
      imdbScore: 'IMDB Score',
      year: 'Year',
      duration: 'Duration',
      director: 'Director',
      genre: 'Genre',
      addToWatchlist: 'Add to Watchlist',
      removeFromWatchlist: 'Remove from Watchlist',
      rate: 'Rate',
      comment: 'Comment',
      watchTrailer: 'Watch Trailer',
      
      // Common
      loading: 'Loading...',
      error: 'An error occurred',
      noResults: 'No results found',
      minutes: 'min',
      
      // Movie details
      cast: 'Cast',
      summary: 'Summary',
      popularity: 'Popularity',
      ranking: 'Ranking',
      comments: 'Comments',
      ratings: 'Ratings'
    },
    tr: {
      // Header
      login: 'Giriş Yap',
      logout: 'Çıkış Yap',
      register: 'Kayıt Ol',
      welcome: 'Hoş Geldiniz',
      profile: 'Profil',
      
      // Search
      searchPlaceholder: 'Film, oyuncu ara...',
      searchMovies: 'Filmler',
      searchActors: 'Oyuncular',
      searchAll: 'Tümü',
      
      // Movies
      popularMovies: 'Popüler Filmler',
      imdbScore: 'IMDB Puanı',
      year: 'Yıl',
      duration: 'Süre',
      director: 'Yönetmen',
      genre: 'Tür',
      addToWatchlist: 'İzleme Listesine Ekle',
      removeFromWatchlist: 'İzleme Listesinden Çıkar',
      rate: 'Puanla',
      comment: 'Yorum Yap',
      watchTrailer: 'Fragman İzle',
      
      // Common
      loading: 'Yükleniyor...',
      error: 'Bir hata oluştu',
      noResults: 'Sonuç bulunamadı',
      minutes: 'dk',
      
      // Movie details
      cast: 'Oyuncular',
      summary: 'Özet',
      popularity: 'Popülerlik',
      ranking: 'Sıralama',
      comments: 'Yorumlar',
      ratings: 'Puanlar'
    }
  };

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.initializeLanguage();
  }

  private initializeLanguage(): void {
    if (isPlatformBrowser(this.platformId)) {
      const savedLanguage = localStorage.getItem('language') as Language;
      const browserLanguage = navigator.language.startsWith('tr') ? 'tr' : 'en';
      const initialLanguage = savedLanguage || browserLanguage;
      
      this.setLanguage(initialLanguage);
    } else {
      // Default to English on server-side
      this.setLanguage('en');
    }
  }

  setLanguage(language: Language): void {
    this.currentLanguageSubject.next(language);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('language', language);
    }
  }

  getCurrentLanguage(): Language {
    return this.currentLanguageSubject.value;
  }

  getTranslations(): Translations {
    return this.translations[this.getCurrentLanguage()];
  }

  translate(key: keyof Translations): string {
    return this.translations[this.getCurrentLanguage()][key];
  }

  getMovieTitle(movie: { title: string, titleTurkish?: string }): string {
    if (this.getCurrentLanguage() === 'tr' && movie.titleTurkish) {
      return movie.titleTurkish;
    }
    return movie.title;
  }

  getMovieSummary(movie: { summary: string, summaryTurkish?: string }): string {
    if (this.getCurrentLanguage() === 'tr' && movie.summaryTurkish) {
      return movie.summaryTurkish;
    }
    return movie.summary;
  }
} 