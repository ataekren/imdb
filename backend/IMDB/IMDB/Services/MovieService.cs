using Microsoft.EntityFrameworkCore;
using IMDB.Data;
using IMDB.Models;
using IMDB.DTOs;
using AutoMapper;

namespace IMDB.Services
{
    public interface IMovieService
    {
        Task<List<MovieSummaryDto>> GetPopularMoviesAsync(int count = 10, int? userId = null);
        Task<MovieDto?> GetMovieByIdAsync(int id, int? userId = null);
        Task<SearchResultDto> SearchAsync(string query, string? searchType = null, int limit = 10, int? userId = null);
        Task<List<MovieSummaryDto>> GetQuickSearchAsync(string query, int limit = 3, int? userId = null);
        Task UpdateMoviePopularityAsync(int movieId);
        Task UpdateAllMoviePopularityAsync();
        Task<List<RatingDistributionDto>> GetRatingDistributionAsync(int movieId);
    }

    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MovieService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MovieSummaryDto>> GetPopularMoviesAsync(int count = 10, int? userId = null)
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.IMDBScore)
                .Take(count)
                .ToListAsync();

            var movieDtos = new List<MovieSummaryDto>();
            foreach (var movie in movies)
            {
                var dto = _mapper.Map<MovieSummaryDto>(movie);
                var ratings = await _context.Ratings.Where(r => r.MovieId == movie.Id).ToListAsync();
                dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                dto.TotalRatings = ratings.Count;
                
                // Add user-specific data if userId is provided
                if (userId.HasValue)
                {
                    dto.IsInWatchlist = await _context.WatchlistItems
                        .AnyAsync(w => w.UserId == userId.Value && w.MovieId == movie.Id);
                    
                    var userRating = await _context.Ratings
                        .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.MovieId == movie.Id);
                    dto.UserRating = userRating?.Score;
                }
                
                movieDtos.Add(dto);
            }

            return movieDtos;
        }

        public async Task<MovieDto?> GetMovieByIdAsync(int id, int? userId = null)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return null;

            // Increment view count
            movie.ViewCount++;
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<MovieDto>(movie);
            
            // Calculate ratings
            var ratings = await _context.Ratings.Where(r => r.MovieId == id).ToListAsync();
            dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
            dto.TotalRatings = ratings.Count;

            // Check if in user's watchlist and get user's rating
            if (userId.HasValue)
            {
                dto.IsInWatchlist = await _context.WatchlistItems
                    .AnyAsync(w => w.UserId == userId.Value && w.MovieId == id);
                
                var userRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.MovieId == id);
                dto.UserRating = userRating?.Score;
            }

            // Map actors
            dto.Actors = movie.MovieActors.Select(ma => new ActorDto
            {
                Id = ma.Actor.Id,
                FirstName = ma.Actor.FirstName,
                LastName = ma.Actor.LastName,
                Biography = ma.Actor.Biography,
                BirthDate = ma.Actor.BirthDate,
                Nationality = ma.Actor.Nationality,
                ProfileImageUrl = ma.Actor.ProfileImageUrl,
                CharacterName = ma.CharacterName
            }).ToList();

            // Update popularity after view synchronously to avoid DbContext concurrency issues
            await UpdateMoviePopularityAsync(id);

            return dto;
        }

        public async Task<SearchResultDto> SearchAsync(string query, string? searchType = null, int limit = 10, int? userId = null)
        {
            var result = new SearchResultDto
            {
                Movies = new List<MovieSummaryDto>(),
                Actors = new List<ActorDto>()
            };

            if (string.IsNullOrWhiteSpace(query))
            {
                return result;
            }

            query = query.ToLowerInvariant().Trim();
            var type = searchType?.ToLowerInvariant().Trim();

            bool searchMovies = type == null || type == "all" || type == "movies" || type == "movie";
            bool searchActors = type == null || type == "all" || type == "actors" || type == "actor";

            if (searchMovies)
            {
                var movies = await _context.Movies
                    .Where(m => 
                        EF.Functions.Like(m.Title.ToLower(), $"%{query}%") || 
                        (m.TitleTurkish != null && EF.Functions.Like(m.TitleTurkish.ToLower(), $"%{query}%")) ||
                        EF.Functions.Like(m.Summary.ToLower(), $"%{query}%") ||
                        (m.SummaryTurkish != null && EF.Functions.Like(m.SummaryTurkish.ToLower(), $"%{query}%")))
                    .OrderByDescending(m => m.IMDBScore)
                    .Take(limit)
                    .ToListAsync();

                foreach (var movie in movies)
                {
                    var dto = _mapper.Map<MovieSummaryDto>(movie);
                    var ratings = await _context.Ratings
                        .Where(r => r.MovieId == movie.Id)
                        .ToListAsync();
                    
                    dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                    dto.TotalRatings = ratings.Count;
                    
                    if (userId.HasValue)
                    {
                        dto.IsInWatchlist = await _context.WatchlistItems
                            .AnyAsync(w => w.UserId == userId.Value && w.MovieId == movie.Id);
                        
                        var userRating = await _context.Ratings
                            .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.MovieId == movie.Id);
                        dto.UserRating = userRating?.Score;
                    }
                    
                    result.Movies.Add(dto);
                }
            }

            if (searchActors)
            {
                var actors = await _context.Actors
                    .Where(a => 
                        EF.Functions.Like(a.FirstName.ToLower(), $"%{query}%") || 
                        EF.Functions.Like(a.LastName.ToLower(), $"%{query}%"))
                    .Take(limit)
                    .ToListAsync();

                foreach (var actor in actors)
                {
                    var actorDto = _mapper.Map<ActorDto>(actor);
                    result.Actors.Add(actorDto);
                }
            }

            return result;
        }

        public async Task<List<MovieSummaryDto>> GetQuickSearchAsync(string query, int limit = 3, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<MovieSummaryDto>();
            }

            query = query.ToLower().Trim();
            
            var movies = await _context.Movies
                .Where(m => 
                    EF.Functions.Like(m.Title.ToLower(), $"%{query}%") || 
                    (m.TitleTurkish != null && EF.Functions.Like(m.TitleTurkish.ToLower(), $"%{query}%")))
                .OrderByDescending(m => m.IMDBScore)
                .Take(limit)
                .ToListAsync();

            var movieDtos = new List<MovieSummaryDto>();
            foreach (var movie in movies)
            {
                var dto = _mapper.Map<MovieSummaryDto>(movie);
                var ratings = await _context.Ratings
                    .Where(r => r.MovieId == movie.Id)
                    .ToListAsync();
                
                dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                dto.TotalRatings = ratings.Count;
                
                if (userId.HasValue)
                {
                    dto.IsInWatchlist = await _context.WatchlistItems
                        .AnyAsync(w => w.UserId == userId.Value && w.MovieId == movie.Id);
                    
                    var userRating = await _context.Ratings
                        .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.MovieId == movie.Id);
                    dto.UserRating = userRating?.Score;
                }
                
                movieDtos.Add(dto);
            }

            return movieDtos;
        }

        public async Task UpdateMoviePopularityAsync(int movieId)
        {
            var movie = await _context.Movies
                .Include(m => m.Ratings)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null) return;

            var ratingCount = movie.Ratings.Count;
            // PopularityScore = ViewCount + (IMDBScore * RatingCount * 100)
            var popularityScore = movie.ViewCount + (movie.IMDBScore * ratingCount * 100);

            movie.PopularityScore = popularityScore;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAllMoviePopularityAsync()
        {
            var movies = await _context.Movies.Include(m => m.Ratings).ToListAsync();

            foreach (var movie in movies)
            {
                var ratingCount = movie.Ratings.Count;
                movie.PopularityScore = movie.ViewCount + (movie.IMDBScore * ratingCount * 100);
            }

            await _context.SaveChangesAsync();

            await UpdatePopularityRankingsAsync();
        }

        private async Task UpdatePopularityRankingsAsync()
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.PopularityScore)
                .ToListAsync();

            for (int i = 0; i < movies.Count; i++)
            {
                movies[i].PopularityRank = i + 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<RatingDistributionDto>> GetRatingDistributionAsync(int movieId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId)
                .ToListAsync();

            var distributionByCountry = ratings
                .GroupBy(r => r.User.Country)
                .Select(g => new RatingDistributionDto
                {
                    Country = g.Key,
                    AverageRating = g.Average(r => r.Score),
                    TotalRatings = g.Count(),
                    RatingCounts = g.GroupBy(r => r.Score)
                                  .ToDictionary(sg => sg.Key, sg => sg.Count())
                })
                .OrderByDescending(d => d.TotalRatings)
                .ToList();

            return distributionByCountry;
        }
    }
} 