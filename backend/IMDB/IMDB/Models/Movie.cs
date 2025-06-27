using System.ComponentModel.DataAnnotations;

namespace IMDB.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? TitleTurkish { get; set; }

        [Required]
        public string Summary { get; set; } = string.Empty;

        public string? SummaryTurkish { get; set; }

        [Range(0, 10)]
        public decimal IMDBScore { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? TrailerUrl { get; set; }

        public int ReleaseYear { get; set; }

        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Director { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        // Popularity metrics
        public int ViewCount { get; set; } = 0;
        public decimal PopularityScore { get; set; } = 0;
        public int PopularityRank { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
} 