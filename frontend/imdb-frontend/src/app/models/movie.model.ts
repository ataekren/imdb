export interface Movie {
  id: number;
  title: string;
  titleTurkish?: string;
  summary: string;
  summaryTurkish?: string;
  imdbScore: number;
  imageUrl?: string;
  trailerUrl?: string;
  releaseYear: number;
  genre: string;
  director: string;
  durationMinutes: number;
  viewCount: number;
  popularityScore: number;
  popularityRank: number;
  actors: Actor[];
  averageRating: number;
  totalRatings: number;
  isInWatchlist: boolean;
  userRating?: number;
}

export interface MovieSummary {
  id: number;
  title: string;
  titleTurkish?: string;
  imdbScore: number;
  imageUrl?: string;
  trailerUrl?: string;
  releaseYear: number;
  genre: string;
  popularityScore: number;
  popularityRank: number;
  averageRating: number;
  totalRatings: number;
  isInWatchlist?: boolean;
  userRating?: number;
}

export interface Actor {
  id: number;
  firstName: string;
  lastName: string;
  biography?: string;
  birthDate?: string;
  nationality?: string;
  profileImageUrl?: string;
  characterName?: string;
}

export interface SearchResult {
  movies: MovieSummary[];
  actors: Actor[];
}

export interface Comment {
  id: number;
  content: string;
  userName: string;
  createdAt: string;
}

export interface RatingDistribution {
  country: string;
  averageRating: number;
  totalRatings: number;
  ratingCounts: { [score: number]: number };
} 