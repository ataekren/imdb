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
                    },
                    new Actor
                    {
                        FirstName = "Robert",
                        LastName = "Downey Jr.",
                        Biography = "American actor and producer known for his role as Iron Man.",
                        BirthDate = new DateTime(1965, 4, 4),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Brad",
                        LastName = "Pitt",
                        Biography = "American actor and film producer.",
                        BirthDate = new DateTime(1963, 12, 18),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Al",
                        LastName = "Pacino",
                        Biography = "American actor and filmmaker.",
                        BirthDate = new DateTime(1940, 4, 25),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Joaquin",
                        LastName = "Phoenix",
                        Biography = "American actor known for his intense performances.",
                        BirthDate = new DateTime(1974, 10, 28),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Ryan",
                        LastName = "Gosling",
                        Biography = "Canadian actor and musician.",
                        BirthDate = new DateTime(1980, 11, 12),
                        Nationality = "Canadian"
                    },
                    new Actor
                    {
                        FirstName = "Emma",
                        LastName = "Stone",
                        Biography = "American actress known for her comedic and dramatic roles.",
                        BirthDate = new DateTime(1988, 11, 6),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Matthew",
                        LastName = "McConaughey",
                        Biography = "American actor and producer.",
                        BirthDate = new DateTime(1969, 11, 4),
                        Nationality = "American"
                    },
                    new Actor
                    {
                        FirstName = "Cillian",
                        LastName = "Murphy",
                        Biography = "Irish actor known for his work in independent and mainstream films.",
                        BirthDate = new DateTime(1976, 5, 25),
                        Nationality = "Irish"
                    },
                    new Actor
                    {
                        FirstName = "Anthony",
                        LastName = "Hopkins",
                        Biography = "Welsh actor known for his portrayal of Hannibal Lecter.",
                        BirthDate = new DateTime(1937, 12, 31),
                        Nationality = "Welsh"
                    },
                    new Actor
                    {
                        FirstName = "Margot",
                        LastName = "Robbie",
                        Biography = "Australian actress and producer.",
                        BirthDate = new DateTime(1990, 7, 2),
                        Nationality = "Australian"
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
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//inception.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=YoHD9XEInc0",
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
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//The%20Dark%20Knight.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
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
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//The%20Shawshank%20Redemption.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=PLl99DlL6b4",
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
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//Pulp%20Fiction.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=s7EdQ4FqbhY",
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
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket/Her.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=dJTU48_yghs",
                        ReleaseYear = 2013,
                        Genre = "Romance",
                        Director = "Spike Jonze",
                        DurationMinutes = 126,
                        ViewCount = 900,
                        PopularityScore = 71.2m,
                        PopularityRank = 5
                    },
                    new Movie
                    {
                        Title = "The Godfather",
                        TitleTurkish = "Baba",
                        Summary = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                        SummaryTurkish = "Organize suç hanedanının yaşlı patriği, gizli imparatorluğunun kontrolünü isteksiz oğluna devreder.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//godfather.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=UaVTIH8mujA",
                        ReleaseYear = 1972,
                        Genre = "Crime",
                        Director = "Francis Ford Coppola",
                        DurationMinutes = 175,
                        ViewCount = 2200,
                        PopularityScore = 94.8m,
                        PopularityRank = 6
                    },
                    new Movie
                    {
                        Title = "Fight Club",
                        TitleTurkish = "Dövüş Kulübü",
                        Summary = "An insomniac office worker and a devil-may-care soap maker form an underground fight club that evolves into an anarchist organization.",
                        SummaryTurkish = "Uykusuz bir ofis çalışanı ve umursamaz bir sabun yapımcısı anarşist bir örgüte dönüşen yeraltı dövüş kulübü kurar.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//fightClub.png",
                        TrailerUrl = "https://www.youtube.com/watch?v=qtRKdVHc-cE",
                        ReleaseYear = 1999,
                        Genre = "Drama",
                        Director = "David Fincher",
                        DurationMinutes = 139,
                        ViewCount = 1750,
                        PopularityScore = 88.2m,
                        PopularityRank = 7
                    },
                    new Movie
                    {
                        Title = "Forrest Gump",
                        TitleTurkish = "Forrest Gump",
                        Summary = "The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal through the eyes of an Alabama man with an IQ of 75.",
                        SummaryTurkish = "Kennedy ve Johnson başkanlıkları, Vietnam Savaşı, Watergate skandalı 75 IQ'ya sahip Alabama'lı bir adamın gözünden anlatılır.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//ForrestGump.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=bLvqoHBptjg",
                        ReleaseYear = 1994,
                        Genre = "Drama",
                        Director = "Robert Zemeckis",
                        DurationMinutes = 142,
                        ViewCount = 1950,
                        PopularityScore = 90.1m,
                        PopularityRank = 8
                    },
                    new Movie
                    {
                        Title = "The Matrix",
                        TitleTurkish = "Matrix",
                        Summary = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                        SummaryTurkish = "Bir bilgisayar korsanı gizemli isyancılardan gerçekliğinin doğası ve kontrolcülerine karşı savaştaki rolü hakkında öğrenir.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//matrix.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=vKQi3bBA1y8",
                        ReleaseYear = 1999,
                        Genre = "Sci-Fi",
                        Director = "The Wachowskis",
                        DurationMinutes = 136,
                        ViewCount = 1680,
                        PopularityScore = 86.7m,
                        PopularityRank = 9
                    },
                    new Movie
                    {
                        Title = "Goodfellas",
                        TitleTurkish = "İyi Çocuklar",
                        Summary = "The story of Henry Hill and his life in the mob, covering his relationship with his wife Karen Hill and his mob partners.",
                        SummaryTurkish = "Henry Hill'in mafyadaki yaşamının hikayesi, karısı Karen Hill ve mafya ortaklarıyla ilişkilerini kapsıyor.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//Goodfellas.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=2ilzidi_J8Q",
                        ReleaseYear = 1990,
                        Genre = "Crime",
                        Director = "Martin Scorsese",
                        DurationMinutes = 146,
                        ViewCount = 1450,
                        PopularityScore = 82.9m,
                        PopularityRank = 10
                    },
                    new Movie
                    {
                        Title = "Interstellar",
                        TitleTurkish = "Yıldızlararası",
                        Summary = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                        SummaryTurkish = "Bir kaşif ekibi, insanlığın hayatta kalmasını sağlamak için uzayda bir solucan deliğinden geçer.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//Interstellar.png",
                        TrailerUrl = "https://www.youtube.com/watch?v=zSWdZVtXT7E",
                        ReleaseYear = 2014,
                        Genre = "Sci-Fi",
                        Director = "Christopher Nolan",
                        DurationMinutes = 169,
                        ViewCount = 1620,
                        PopularityScore = 84.3m,
                        PopularityRank = 11
                    },
                    new Movie
                    {
                        Title = "La La Land",
                        TitleTurkish = "La La Land",
                        Summary = "While navigating their careers in Los Angeles, a pianist and an actress fall in love while attempting to reconcile their aspirations.",
                        SummaryTurkish = "Los Angeles'ta kariyerlerini sürdürürken, bir piyanist ve aktris aspirasyonlarını uzlaştırmaya çalışırken aşık olur.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//LaLaLand.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=0pdqf4P9MB8",
                        ReleaseYear = 2016,
                        Genre = "Musical",
                        Director = "Damien Chazelle",
                        DurationMinutes = 128,
                        ViewCount = 1280,
                        PopularityScore = 79.5m,
                        PopularityRank = 12
                    },
                    new Movie
                    {
                        Title = "Joker",
                        TitleTurkish = "Joker",
                        Summary = "A gritty character study of Arthur Fleck, a man disregarded by society, and a broader cautionary tale.",
                        SummaryTurkish = "Toplum tarafından görmezden gelinen Arthur Fleck'in karakterinin çetin incelemesi ve daha geniş bir uyarı hikayesi.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//Joker.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=zAGVQLHvwOY",
                        ReleaseYear = 2019,
                        Genre = "Drama",
                        Director = "Todd Phillips",
                        DurationMinutes = 122,
                        ViewCount = 1890,
                        PopularityScore = 87.6m,
                        PopularityRank = 13
                    },
                    new Movie
                    {
                        Title = "Oppenheimer",
                        TitleTurkish = "Oppenheimer",
                        Summary = "The story of American scientist J. Robert Oppenheimer and his role in the development of the atomic bomb.",
                        SummaryTurkish = "Amerikalı bilim insanı J. Robert Oppenheimer'ın hikayesi ve atom bombasının geliştirilmesindeki rolü.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//Oppenheimer.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=uYPbbksJxIg",
                        ReleaseYear = 2023,
                        Genre = "Biography",
                        Director = "Christopher Nolan",
                        DurationMinutes = 180,
                        ViewCount = 1340,
                        PopularityScore = 81.7m,
                        PopularityRank = 14
                    },
                    new Movie
                    {
                        Title = "The Silence of the Lambs",
                        TitleTurkish = "Kuzuların Sessizliği",
                        Summary = "A young FBI cadet must receive the help of an incarcerated and manipulative cannibal killer to help catch another serial killer.",
                        SummaryTurkish = "Genç bir FBI öğrencisi, başka bir seri katili yakalamak için hapsedilmiş ve manipülatif yamyam katilin yardımını almalıdır.",
                        IMDBScore = 0,
                        ImageUrl = "https://gqxlxvxezmvornwhmmms.supabase.co/storage/v1/object/public/imdb-bucket//TheSilenceoftheLambs.jpg",
                        TrailerUrl = "https://www.youtube.com/watch?v=6iB21hsprAQ",
                        ReleaseYear = 1991,
                        Genre = "Thriller",
                        Director = "Jonathan Demme",
                        DurationMinutes = 118,
                        ViewCount = 1520,
                        PopularityScore = 83.4m,
                        PopularityRank = 15
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
                    new MovieActor { MovieId = movies[4].Id, ActorId = actors[5].Id, CharacterName = "Samantha (voice)" },

                    // The Godfather cast
                    new MovieActor { MovieId = movies[5].Id, ActorId = actors[9].Id, CharacterName = "Don Vito Corleone" },

                    // Fight Club cast
                    new MovieActor { MovieId = movies[6].Id, ActorId = actors[7].Id, CharacterName = "Tyler Durden" },

                    // The Matrix cast (using existing actors)
                    new MovieActor { MovieId = movies[8].Id, ActorId = actors[4].Id, CharacterName = "Morpheus" },

                    // Goodfellas cast
                    new MovieActor { MovieId = movies[9].Id, ActorId = actors[6].Id, CharacterName = "Henry Hill" },

                    // Interstellar cast
                    new MovieActor { MovieId = movies[10].Id, ActorId = actors[12].Id, CharacterName = "Cooper" },
                    new MovieActor { MovieId = movies[10].Id, ActorId = actors[13].Id, CharacterName = "Mann" },

                    // La La Land cast
                    new MovieActor { MovieId = movies[11].Id, ActorId = actors[10].Id, CharacterName = "Sebastian" },
                    new MovieActor { MovieId = movies[11].Id, ActorId = actors[11].Id, CharacterName = "Mia" },

                    // Joker cast
                    new MovieActor { MovieId = movies[12].Id, ActorId = actors[8].Id, CharacterName = "Arthur Fleck / Joker" },

                    // Oppenheimer cast
                    new MovieActor { MovieId = movies[13].Id, ActorId = actors[13].Id, CharacterName = "J. Robert Oppenheimer" },

                    // The Silence of the Lambs cast
                    new MovieActor { MovieId = movies[14].Id, ActorId = actors[14].Id, CharacterName = "Dr. Hannibal Lecter" }
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