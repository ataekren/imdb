using Microsoft.EntityFrameworkCore;
using IMDB.Data;
using IMDB.Models;
using IMDB.DTOs;
using AutoMapper;

namespace IMDB.Services
{
    public interface IMovieService
    {
        Task<List<MovieSummaryDto>> GetPopularMoviesAsync(int count = 10);
        Task<MovieDto?> GetMovieByIdAsync(int id, int? userId = null);
        Task<SearchResultDto> SearchAsync(string query, string? searchType = null, int limit = 10);
        Task<List<MovieSummaryDto>> GetQuickSearchAsync(string query, int limit = 3);
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

        public async Task<List<MovieSummaryDto>> GetPopularMoviesAsync(int count = 10)
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.PopularityScore)
                .Take(count)
                .ToListAsync();

            var movieDtos = new List<MovieSummaryDto>();
            foreach (var movie in movies)
            {
                var dto = _mapper.Map<MovieSummaryDto>(movie);
                var ratings = await _context.Ratings.Where(r => r.MovieId == movie.Id).ToListAsync();
                dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                dto.TotalRatings = ratings.Count;
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

            // Update popularity after view
            _ = Task.Run(() => UpdateMoviePopularityAsync(id));

            return dto;
        }

        public async Task<SearchResultDto> SearchAsync(string query, string? searchType = null, int limit = 10)
        {
            var result = new SearchResultDto();

            if (searchType == null || searchType == "title")
            {
                var movies = await _context.Movies
                    .Where(m => m.Title.Contains(query) || 
                               (m.TitleTurkish != null && m.TitleTurkish.Contains(query)) ||
                               m.Summary.Contains(query) ||
                               (m.SummaryTurkish != null && m.SummaryTurkish.Contains(query)))
                    .OrderByDescending(m => m.PopularityScore)
                    .Take(limit)
                    .ToListAsync();

                foreach (var movie in movies)
                {
                    var dto = _mapper.Map<MovieSummaryDto>(movie);
                    var ratings = await _context.Ratings.Where(r => r.MovieId == movie.Id).ToListAsync();
                    dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                    dto.TotalRatings = ratings.Count;
                    result.Movies.Add(dto);
                }
            }

            if (searchType == null || searchType == "actor")
            {
                result.Actors = await _context.Actors
                    .Where(a => a.FirstName.Contains(query) || a.LastName.Contains(query))
                    .Take(limit)
                    .Select(a => _mapper.Map<ActorDto>(a))
                    .ToListAsync();
            }

            return result;
        }

        public async Task<List<MovieSummaryDto>> GetQuickSearchAsync(string query, int limit = 3)
        {
            var movies = await _context.Movies
                .Where(m => m.Title.Contains(query) || 
                           (m.TitleTurkish != null && m.TitleTurkish.Contains(query)))
                .OrderByDescending(m => m.PopularityScore)
                .Take(limit)
                .ToListAsync();

            var movieDtos = new List<MovieSummaryDto>();
            foreach (var movie in movies)
            {
                var dto = _mapper.Map<MovieSummaryDto>(movie);
                var ratings = await _context.Ratings.Where(r => r.MovieId == movie.Id).ToListAsync();
                dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                dto.TotalRatings = ratings.Count;
                movieDtos.Add(dto);
            }

            return movieDtos;
        }

        public async Task UpdateMoviePopularityAsync(int movieId)
        {
            var movie = await _context.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null) return;

            // Calculate popularity score based on multiple factors  
            var avgRating = movie.Ratings.Any() ? movie.Ratings.Average(r => r.Score) : 0;
            var ratingCount = movie.Ratings.Count;
            var commentCount = movie.Comments.Count;
            var viewCount = movie.ViewCount;

            // Convert avgRating to decimal to match the type of other operands  
            var popularityScore =
                ((decimal)avgRating * 0.3m) +                           // 30% rating quality  
                (Math.Min(ratingCount / 10.0m, 10) * 0.25m) +          // 25% rating quantity (capped)  
                (Math.Min(commentCount / 5.0m, 10) * 0.15m) +          // 15% comments (capped)  
                (Math.Min(viewCount / 100.0m, 20) * 0.3m);             // 30% views (capped)  

            movie.PopularityScore = popularityScore;
            await _context.SaveChangesAsync();

            // Update popularity rankings  
            await UpdatePopularityRankingsAsync();
        }

        public async Task UpdateAllMoviePopularityAsync()
        {
            var movies = await _context.Movies.ToListAsync();
            
            foreach (var movie in movies)
            {
                await UpdateMoviePopularityAsync(movie.Id);
            }
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