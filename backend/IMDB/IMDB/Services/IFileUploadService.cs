namespace IMDB.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadProfilePictureAsync(IFormFile file, string userId);
        Task<bool> DeleteFileAsync(string fileName);
    }
} 