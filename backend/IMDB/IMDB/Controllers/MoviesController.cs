using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AutoMapper;
using IMDB.Data;
using IMDB.Models;
using IMDB.DTOs;
using IMDB.Services;

namespace IMDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public MoviesController(ApplicationDbContext context, IMovieService movieService, IMapper mapper)
        {
            _context = context;
            _movieService = movieService;
            _mapper = mapper;
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies([FromQuery] int count = 10)
        {
            var userId = GetUserId();
            var movies = await _movieService.GetPopularMoviesAsync(count, userId);
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var userId = GetUserId();
            var movie = await _movieService.GetMovieByIdAsync(id, userId);
            
            if (movie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            return Ok(movie);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string? type = null, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Query is required" });
            }

            var userId = GetUserId();
            var results = await _movieService.SearchAsync(query, type, limit, userId);
            return Ok(results);
        }

        [HttpGet("quick-search")]
        public async Task<IActionResult> QuickSearch([FromQuery] string query, [FromQuery] int limit = 3)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            {
                return BadRequest(new { message = "Query must be at least 3 characters long" });
            }

            var userId = GetUserId();
            var movies = await _movieService.GetQuickSearchAsync(query, limit, userId);
            return Ok(movies);
        }

        [HttpGet("{id}/rating-distribution")]
        public async Task<IActionResult> GetRatingDistribution(int id)
        {
            var distribution = await _movieService.GetRatingDistributionAsync(id);
            return Ok(distribution);
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.MovieId == id)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    UserName = $"{c.User.FirstName} {c.User.LastName}",
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost("{id}/rate")]
        [Authorize]
        public async Task<IActionResult> RateMovie(int id, [FromBody] RatingDto ratingDto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                // Check if movie exists
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound(new { message = "Movie not found" });
                }

                // Check if user already rated this movie
                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.MovieId == id);

                if (existingRating != null)
                {
                    // Update existing rating
                    existingRating.Score = ratingDto.Score;
                    existingRating.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new rating
                    var rating = _mapper.Map<Rating>(ratingDto);
                    rating.UserId = userId.Value;
                    rating.MovieId = id;
                    _context.Ratings.Add(rating);
                }

                await _context.SaveChangesAsync();

                // Update movie's IMDb score with recalculated average
                await UpdateMovieIMDbScoreAsync(id);

                // Recalculate popularity for all movies synchronously to avoid DbContext concurrency issues
                await _movieService.UpdateAllMoviePopularityAsync();

                // Get updated movie data to return
                var updatedMovie = await _context.Movies.FindAsync(id);
                var allRatings = await _context.Ratings.Where(r => r.MovieId == id).ToListAsync();
                
                return Ok(new { 
                    message = "Rating saved successfully",
                    updatedMovieData = new {
                        id = updatedMovie.Id,
                        imdbScore = updatedMovie.IMDBScore,
                        averageRating = allRatings.Any() ? allRatings.Average(r => r.Score) : 0,
                        totalRatings = allRatings.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to save rating", error = ex.Message });
            }
        }

        [HttpPost("{id}/comment")]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentDto commentDto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                // Check if movie exists
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound(new { message = "Movie not found" });
                }

                var comment = _mapper.Map<Comment>(commentDto);
                comment.UserId = userId.Value;
                comment.MovieId = id;

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // Update movie popularity
                _ = Task.Run(() => _movieService.UpdateMoviePopularityAsync(id));

                return Ok(new { message = "Comment added successfully", commentId = comment.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add comment", error = ex.Message });
            }
        }

        [HttpPost("{id}/watchlist")]
        [Authorize]
        public async Task<IActionResult> AddToWatchlist(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                // Check if movie exists
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound(new { message = "Movie not found" });
                }

                // Check if already in watchlist
                var existingItem = await _context.WatchlistItems
                    .FirstOrDefaultAsync(w => w.UserId == userId.Value && w.MovieId == id);

                if (existingItem != null)
                {
                    return BadRequest(new { message = "Movie is already in watchlist" });
                }

                var watchlistItem = new WatchlistItem
                {
                    UserId = userId.Value,
                    MovieId = id
                };

                _context.WatchlistItems.Add(watchlistItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Movie added to watchlist" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add to watchlist", error = ex.Message });
            }
        }

        [HttpDelete("{id}/watchlist")]
        [Authorize]
        public async Task<IActionResult> RemoveFromWatchlist(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var watchlistItem = await _context.WatchlistItems
                    .FirstOrDefaultAsync(w => w.UserId == userId.Value && w.MovieId == id);

                if (watchlistItem == null)
                {
                    return NotFound(new { message = "Movie not found in watchlist" });
                }

                _context.WatchlistItems.Remove(watchlistItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Movie removed from watchlist" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to remove from watchlist", error = ex.Message });
            }
        }

        [HttpGet("watchlist")]
        [Authorize]
        public async Task<IActionResult> GetWatchlist([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var watchlistMovies = await _context.WatchlistItems
                .Include(w => w.Movie)
                .Where(w => w.UserId == userId.Value)
                .OrderByDescending(w => w.AddedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => w.Movie)
                .ToListAsync();

            var movieDtos = new List<MovieSummaryDto>();
            foreach (var movie in watchlistMovies)
            {
                var dto = _mapper.Map<MovieSummaryDto>(movie);
                var ratings = await _context.Ratings.Where(r => r.MovieId == movie.Id).ToListAsync();
                dto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
                dto.TotalRatings = ratings.Count;
                movieDtos.Add(dto);
            }

            return Ok(movieDtos);
        }

        [HttpPost("update-popularity")]
        [Authorize] // In production, this should be restricted to admin users
        public async Task<IActionResult> UpdateAllMoviePopularity()
        {
            try
            {
                await _movieService.UpdateAllMoviePopularityAsync();
                return Ok(new { message = "Movie popularity updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update popularity", error = ex.Message });
            }
        }

        private async Task UpdateMovieIMDbScoreAsync(int movieId)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null) return;

            var ratings = await _context.Ratings.Where(r => r.MovieId == movieId).ToListAsync();
            
            if (ratings.Any())
            {
                var averageRating = ratings.Average(r => r.Score);
                movie.IMDBScore = (decimal)averageRating;
                movie.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
    }
} 