using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace IMDB.Services
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseToken> VerifyIdTokenAsync(string idToken);
    }

    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly IConfiguration _configuration;

        public FirebaseAuthService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            
            // Initialize Firebase Admin SDK
            if (FirebaseApp.DefaultInstance == null)
            {
                var serviceAccountPath = Path.Combine(environment.ContentRootPath, "firebase-adminsdk.json");
                
                if (File.Exists(serviceAccountPath))
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountPath),
                        ProjectId = _configuration["Firebase:ProjectId"]
                    });
                }
                else
                {
                    // Fallback to environment variable
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.GetApplicationDefault(),
                        ProjectId = _configuration["Firebase:ProjectId"]
                    });
                }
            }
        }

        public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Invalid Firebase token", ex);
            }
        }
    }
} 