# 🎬 IMDB Web Project

A modern, full-stack movie database application built with Angular and .NET Core.

## 🔗 Links  
- [Demo Video Link](https://www.youtube.com/watch?v=Ski6BP0eWi8)
- [Frontend Deployment](https://imdb-ata.vercel.app)
- [GitHub Repository Link](https://github.com/ataekren/imdb)

## Features

### For Users
- **Movie Discovery**
  - Browse popular movies with detailed information
  - Advanced search functionality for movies and actors
  - Watch trailers and view movie details
  - Multilingual support (English and Turkish)

- **User Interactions**
  - Rate movies (0-10 scale)
  - Add/remove movies to/from watchlist
  - Comment on movies
  - View movie statistics and ratings

- **Authentication**
  - Email/password registration and login
  - Google OAuth integration
  - Secure JWT-based authentication

### Technical Features
- Responsive design that adapts to different screen sizes
- Real-time language switching (English/Turkish)
- Client-side caching for better performance
- Server-side data pagination
- Secure file upload for movie images
- RESTful API architecture

## 🏗 Architecture

### Frontend (Angular)
- **Components**
  - Movie Slider: Displays popular movies in a carousel
  - Movie Detail: Shows comprehensive movie information
  - Rating Popup: Handles user ratings
  - Search Results: Displays search findings
  - Authentication: Handles user login/registration

- **Services**
  - API Service: Handles backend communication
  - Auth Service: Manages authentication state
  - Language Service: Handles internationalization
  
### Backend (.NET Core)
- **Controllers**
  - MoviesController: Handles movie-related operations
  - AuthController: Manages authentication
  
- **Services**
  - MovieService: Business logic for movies
  - JwtService: Token generation and validation
  - FirebaseAuthService: Google authentication
  - SupabaseFileUploadService: File storage

## 📊 Data Model

### Core Entities

#### Movie
- Basic Info: Title (multilingual), Summary, Release Year, Genre, Director
- Media: Image URL, Trailer URL
- Metrics: IMDB Score, View Count, Popularity Score
- Relations: Actors, Ratings, Comments, Watchlist Items

#### User
- Profile: Email, First/Last Name, Location
- Auth: Password Hash, Google ID
- Activity: Ratings, Comments, Watchlist

#### Supporting Entities
- Rating: Links User and Movie with a score
- Comment: User comments on movies
- WatchlistItem: Movies saved by users
- MovieActor: Many-to-many relationship between Movies and Actors

## 🚀 Getting Started

### Prerequisites
- Node.js (v18+)
- .NET Core SDK (v7.0+)
- SQL Server
- Firebase account (for Google Auth)
- Supabase account (for file storage)


## 🔒 Security

- JWT-based authentication
- Password hashing using bcrypt
- HTTPS enforcement in production
- CORS configuration
- Input validation and sanitization
- File upload restrictions and validation


## 🎨 Design Decisions

1. **Multilingual Support**
   - Separate fields for English and Turkish content
   - Client-side language switching without page reload
   - Persistent language preference

2. **Movie Rating System**
   - 0-10 scale for consistency with IMDB
   - Aggregate scores with weighted averages
   - Real-time updates of movie ratings

3. **Performance Optimization**
   - Client-side caching of movie data
   - Lazy loading of images
   - Pagination for large datasets
   - Optimized database queries

4. **User Experience**
   - Responsive design for all screen sizes
   - Intuitive navigation
   - Real-time feedback on user actions
   - Smooth animations and transitions
