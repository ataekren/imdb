using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMDB.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? GoogleId { get; set; }

        [MaxLength(500)]
        public string? ProfilePicture { get; set; }

        public bool IsGoogleAuth { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
    }
} 