using IMDB.Models;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();

                // Check if data already exists
                if (await context.Movies.AnyAsync())
                {
                    return; // Database has been seeded
                }

                // Seed Actors
                var actors = new List<Actor>
                {
                    new Actor
                    {
                        FirstName = "Leonardo",
                        LastName = "DiCaprio",
                        Biography = "American actor and film producer known for his work in biographical and period films.",
                        BirthDate = new DateTime(1974, 11, 11),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Marion",
                        LastName = "Cotillard",
                        Biography = "French actress, singer, and environmentalist.",
                        BirthDate = new DateTime(1975, 9, 30),
                        Nationality = "French"
                    },
                    new Actor
                    {
                        FirstName = "Tom",
                        LastName = "Hardy",
                        Biography = "English actor and producer.",
                        BirthDate = new DateTime(1977, 9, 15),
                        Nationality = "British"
                    },
                    new Actor
                    {
                        FirstName = "Christian",
                        LastName = "Bale",
                        Biography = "English actor known for his intense method acting style.",
                        BirthDate = new DateTime(1974, 1, 30),
                        Nationality = "British"
                    },
                    new Actor
                    {
                        FirstName = "Morgan",
                        LastName = "Freeman",
                        Biography = "American actor, director, and narrator known for his distinctive deep voice.",
                        BirthDate = new DateTime(1937, 6, 1),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Scarlett",
                        LastName = "Johansson",
                        Biography = "American actress and singer.",
                        BirthDate = new DateTime(1984, 11, 22),
                        Nationality = "American"
                    }
                };

                await context.Actors.AddRangeAsync(actors);
                await context.SaveChangesAsync();

                // Seed Movies
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "Inception",
                        TitleTurkish = "Başlangıç",
                        Summary = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                        SummaryTurkish = "Rüya paylaşım teknolojisini kullanarak kurumsal sırları çalan bir hırsız, bir CEO'nun zihnine bir fikir yerleştirme görevini alır.",
                        IMDBScore = 8.8m,
                        ImageUrl = "https://example.com/inception.jpg",
                        TrailerUrl = "https://example.com/inception-trailer.mp4",
                        ReleaseYear = 2010,
                        Genre = "Sci-Fi",
                        Director = "Christopher Nolan",
                        DurationMinutes = 148,
                        ViewCount = 1500,
                        PopularityScore = 85.5m,
                        PopularityRank = 1
                    },
                    new Movie
                    {
                        Title = "The Dark Knight",
                        TitleTurkish = "Kara Şövalye",
                        Summary = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests.",
                        SummaryTurkish = "Joker olarak bilinen tehdit Gotham halkına kaos saçarken, Batman en büyük psikolojik ve fiziksel testlerden birini kabul etmek zorundadır.",
                        IMDBScore = 9.0m,
                        ImageUrl = "https://example.com/dark-knight.jpg",
                        TrailerUrl = "https://example.com/dark-knight-trailer.mp4",
                        ReleaseYear = 2008,
                        Genre = "Action",
                        Director = "Christopher Nolan",
                        DurationMinutes = 152,
                        ViewCount = 2000,
                        PopularityScore = 92.3m,
                        PopularityRank = 2
                    },
                    new Movie
                    {
                        Title = "The Shawshank Redemption",
                        TitleTurkish = "Esaretin Bedeli",
                        Summary = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                        SummaryTurkish = "İki mahkum yıllar boyunca bağ kurar, ortak nezaket eylemleri yoluyla teselli ve sonunda kurtuluş bulur.",
                        IMDBScore = 9.3m,
                        ImageUrl = "https://example.com/shawshank.jpg",
                        TrailerUrl = "https://example.com/shawshank-trailer.mp4",
                        ReleaseYear = 1994,
                        Genre = "Drama",
                        Director = "Frank Darabont",
                        DurationMinutes = 142,
                        ViewCount = 1800,
                        PopularityScore = 89.7m,
                        PopularityRank = 3
                    },
                    new Movie
                    {
                        Title = "Pulp Fiction",
                        TitleTurkish = "Ucuz Roman",
                        Summary = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
                        SummaryTurkish = "İki mafya tetikçisi, bir boksör, bir gangster ve karısının hayatları şiddet ve kurtuluş öykülerinde iç içe geçer.",
                        IMDBScore = 8.9m,
                        ImageUrl = "https://example.com/pulp-fiction.jpg",
                        TrailerUrl = "https://example.com/pulp-fiction-trailer.mp4",
                        ReleaseYear = 1994,
                        Genre = "Crime",
                        Director = "Quentin Tarantino",
                        DurationMinutes = 154,
                        ViewCount = 1200,
                        PopularityScore = 78.4m,
                        PopularityRank = 4
                    },
                    new Movie
                    {
                        Title = "Her",
                        TitleTurkish = "O",
                        Summary = "In a near future, a lonely writer develops an unlikely relationship with an operating system designed to meet his every need.",
                        SummaryTurkish = "Yakın gelecekte, yalnız bir yazar, her ihtiyacını karşılamak için tasarlanmış bir işletim sistemiyle beklenmedik bir ilişki geliştirir.",
                        IMDBScore = 8.0m,
                        ImageUrl = "https://example.com/her.jpg",
                        TrailerUrl = "https://example.com/her-trailer.mp4",
                        ReleaseYear = 2013,
                        Genre = "Romance",
                        Director = "Spike Jonze",
                        DurationMinutes = 126,
                        ViewCount = 900,
                        PopularityScore = 71.2m,
                        PopularityRank = 5
                    }
                };

                await context.Movies.AddRangeAsync(movies);
                await context.SaveChangesAsync();

                // Seed Movie-Actor relationships
                var movieActors = new List<MovieActor>
                {
                    // Inception cast
                    new MovieActor { MovieId = movies[0].Id, ActorId = actors[0].Id, CharacterName = "Dom Cobb" },
                    new MovieActor { MovieId = movies[0].Id, ActorId = actors[1].Id, CharacterName = "Mal" },
                    new MovieActor { MovieId = movies[0].Id, ActorId = actors[2].Id, CharacterName = "Eames" },
                    
                    // The Dark Knight cast
                    new MovieActor { MovieId = movies[1].Id, ActorId = actors[3].Id, CharacterName = "Batman" },
                    new MovieActor { MovieId = movies[1].Id, ActorId = actors[4].Id, CharacterName = "Lucius Fox" },
                    
                    // Her cast
                    new MovieActor { MovieId = movies[4].Id, ActorId = actors[5].Id, CharacterName = "Samantha (voice)" }
                };

                await context.MovieActors.AddRangeAsync(movieActors);
                await context.SaveChangesAsync();

                Console.WriteLine("Database seeded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
                throw;
            }
        }
    }
} 