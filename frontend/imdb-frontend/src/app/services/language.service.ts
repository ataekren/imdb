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
  popularMoviesDescription: string;
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
  statistics: string;
  views: string;
  yourRating: string;
  rated: string;
  goBack: string;
  
  // Auth
  email: string;
  password: string;
  signUp: string;
  loginWithGoogle: string;
  forgotPassword: string;
  rememberMe: string;
  fillAllFields: string;
  googleLoginFailed: string;
  googleSignInFailed: string;
  loginFailed: string;
  dontHaveAccount: string;
  alreadyHaveAccount: string;
  
  // Countries and Cities
  turkey: string;
  istanbul: string;
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
      popularMovies: 'Top 10 on IMDB this week',
      popularMoviesDescription: 'Discover the most popular movies with ratings, trailers, and reviews',
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
      summary: 'Synopsis',
      popularity: 'Popularity',
      ranking: 'Popularity',
      comments: 'Comments',
      ratings: 'Ratings',
      statistics: 'Statistics',
      views: 'Views',
      yourRating: 'Your Rating',
      rated: 'Rated',
      goBack: 'Back',
      
      // Auth
      email: 'Email',
      password: 'Password',
      signUp: 'Sign Up',
      loginWithGoogle: 'Login with Google',
      forgotPassword: 'Forgot Password',
      rememberMe: 'Remember Me',
      fillAllFields: 'Fill all fields',
      googleLoginFailed: 'Google Login Failed',
      googleSignInFailed: 'Google Sign In Failed',
      loginFailed: 'Login Failed',
      dontHaveAccount: "Don't have an account?",
      alreadyHaveAccount: 'Already have an account?',
      
      // Countries and Cities
      turkey: 'Turkey',
      istanbul: 'Istanbul'
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
      popularMoviesDescription: 'En popüler filmleri puanları, fragmanları ve yorumlarıyla keşfedin',
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
      ranking: 'Popülerlik',
      comments: 'Yorumlar',
      ratings: 'Puan',
      statistics: 'İstatistikler',
      views: 'Görüntülenme',
      yourRating: 'Puanınız',
      rated: 'Puanlandı',
      goBack: 'Geri',
      
      // Auth
      email: 'Email',
      password: 'Şifre',
      signUp: 'Kayıt Ol',
      loginWithGoogle: 'Google ile Giriş Yap',
      forgotPassword: 'Şifremi Unuttum',
      rememberMe: 'Beni Hatırla',
      fillAllFields: 'Tüm alanları doldurun',
      googleLoginFailed: 'Google Girişi Başarısız',
      googleSignInFailed: 'Google Girişi Başarısız',
      loginFailed: 'Giriş Başarısız',
      dontHaveAccount: 'Hesabınız yok mu?',
      alreadyHaveAccount: 'Zaten bir hesabınız var mı?',
      
      // Countries and Cities
      turkey: 'Türkiye',
      istanbul: 'İstanbul'
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