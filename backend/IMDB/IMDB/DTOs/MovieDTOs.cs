using System.ComponentModel.DataAnnotations;

namespace IMDB.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleTurkish { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string? SummaryTurkish { get; set; }
        public decimal IMDBScore { get; set; }
        public string? ImageUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public int ReleaseYear { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int ViewCount { get; set; }
        public decimal PopularityScore { get; set; }
        public int PopularityRank { get; set; }
        public List<ActorDto> Actors { get; set; } = new List<ActorDto>();
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public bool IsInWatchlist { get; set; }
        public int? UserRating { get; set; }
    }

    public class MovieSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleTurkish { get; set; }
        public decimal IMDBScore { get; set; }
        public string? ImageUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public int ReleaseYear { get; set; }
        public string Genre { get; set; } = string.Empty;
        public decimal PopularityScore { get; set; }
        public int PopularityRank { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public bool IsInWatchlist { get; set; }
        public int? UserRating { get; set; }
    }

    public class ActorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Nationality { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CharacterName { get; set; }
    }

    public class SearchResultDto
    {
        public List<MovieSummaryDto> Movies { get; set; } = new List<MovieSummaryDto>();
        public List<ActorDto> Actors { get; set; } = new List<ActorDto>();
    }

    public class RatingDto
    {
        [Required]
        [Range(1, 10)]
        public int Score { get; set; }
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCommentDto
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }

    public class RatingDistributionDto
    {
        public string Country { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public Dictionary<int, int> RatingCounts { get; set; } = new Dictionary<int, int>();
    }
} 