import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Movie, 
  MovieSummary, 
  SearchResult, 
  Comment,
  RatingDistribution
} from '../models/movie.model';
import { 
  AuthResponse, 
  LoginRequest, 
  RegisterRequest, 
  GoogleAuthRequest,
  RatingRequest,
  CommentRequest 
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  //private baseUrl = 'https://localhost:7053/api';
  private baseUrl = 'https://imdb-web.azurewebsites.net/api';

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    let token: string | null = null;
    if (typeof localStorage !== 'undefined') {
      token = localStorage.getItem('token');
    }
    return new HttpHeaders({
      'Content-Type': 'application/json',
      ...(token && { 'Authorization': `Bearer ${token}` })
    });
  }

  private getAuthHeaders(): HttpHeaders {
    let token: string | null = null;
    if (typeof localStorage !== 'undefined') {
      token = localStorage.getItem('token');
    }
    return new HttpHeaders({
      ...(token && { 'Authorization': `Bearer ${token}` })
    });
  }

  // Authentication endpoints
  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/register`, data);
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/login`, data);
  }

  googleAuth(data: GoogleAuthRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/google`, data);
  }

  uploadProfilePicture(file: File): Observable<{message: string, user: any}> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<{message: string, user: any}>(
      `${this.baseUrl}/auth/upload-profile-picture`, 
      formData,
      { headers: this.getAuthHeaders() }
    );
  }

  // Movie endpoints
  getPopularMovies(count: number = 10): Observable<MovieSummary[]> {
    return this.http.get<MovieSummary[]>(
      `${this.baseUrl}/movies/popular?count=${count}`,
      { headers: this.getHeaders() }
    );
  }

  getMovie(id: number): Observable<Movie> {
    return this.http.get<Movie>(`${this.baseUrl}/movies/${id}`, {
      headers: this.getHeaders()
    });
  }

  searchMovies(query: string, type?: string, limit: number = 10): Observable<SearchResult> {
    let url = `${this.baseUrl}/movies/search?query=${encodeURIComponent(query)}&limit=${limit}`;
    if (type) {
      url += `&type=${type}`;
    }
    return this.http.get<SearchResult>(url, { headers: this.getHeaders() });
  }

  quickSearch(query: string, limit: number = 3): Observable<MovieSummary[]> {
    return this.http.get<MovieSummary[]>(
      `${this.baseUrl}/movies/quick-search?query=${encodeURIComponent(query)}&limit=${limit}`,
      { headers: this.getHeaders() }
    );
  }

  // User actions (require authentication)
  rateMovie(movieId: number, rating: RatingRequest): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/movies/${movieId}/rate`, 
      rating,
      { headers: this.getHeaders() }
    );
  }

  commentOnMovie(movieId: number, comment: CommentRequest): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/movies/${movieId}/comment`, 
      comment,
      { headers: this.getHeaders() }
    );
  }

  addToWatchlist(movieId: number): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/movies/${movieId}/watchlist`, 
      {},
      { headers: this.getHeaders() }
    );
  }

  removeFromWatchlist(movieId: number): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}/movies/${movieId}/watchlist`,
      { headers: this.getHeaders() }
    );
  }

  getWatchlist(page: number = 1, pageSize: number = 10): Observable<MovieSummary[]> {
    return this.http.get<MovieSummary[]>(
      `${this.baseUrl}/movies/watchlist?page=${page}&pageSize=${pageSize}`,
      { headers: this.getHeaders() }
    );
  }

  getMovieComments(movieId: number, page: number = 1, pageSize: number = 10): Observable<Comment[]> {
    return this.http.get<Comment[]>(
      `${this.baseUrl}/movies/${movieId}/comments?page=${page}&pageSize=${pageSize}`
    );
  }

  getRatingDistribution(movieId: number): Observable<RatingDistribution[]> {
    return this.http.get<RatingDistribution[]>(
      `${this.baseUrl}/movies/${movieId}/rating-distribution`,
      { headers: this.getHeaders() }
    );
  }
} 