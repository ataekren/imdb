using System.ComponentModel.DataAnnotations;

namespace IMDB.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Biography { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
} 