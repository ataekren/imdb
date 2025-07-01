using Supabase;
using Supabase.Storage;

namespace IMDB.Services
{
    public class SupabaseFileUploadService : IFileUploadService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly IConfiguration _configuration;
        private const string BucketName = "imdb-bucket";

        public SupabaseFileUploadService(Supabase.Client supabaseClient, IConfiguration configuration)
        {
            _supabaseClient = supabaseClient;
            _configuration = configuration;
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Invalid file type. Only JPEG, PNG, GIF and WebP are allowed.");

            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size must be less than 5MB");

            try
            {
                // Generate unique filename
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";

                // Convert IFormFile to byte array
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Upload to Supabase
                await _supabaseClient.Storage
                    .From(BucketName)
                    .Upload(fileBytes, fileName, new Supabase.Storage.FileOptions
                    {
                        ContentType = file.ContentType,
                        Upsert = false
                    });

                // Get public URL
                var publicUrl = _supabaseClient.Storage
                    .From(BucketName)
                    .GetPublicUrl(fileName);

                return publicUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload file: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                await _supabaseClient.Storage
                    .From(BucketName)
                    .Remove(new List<string> { fileName });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 