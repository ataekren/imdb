export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  country: string;
  city: string;
  isGoogleAuth: boolean;
  createdAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  country: string;
  city: string;
}

export interface GoogleAuthRequest {
  idToken: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface RatingRequest {
  score: number;
}

export interface CommentRequest {
  content: string;
} 