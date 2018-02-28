using Firebase.Database;
using SupClubLib.Services;

namespace SupClubLib.Repositories
{
    public class FirebaseRepository
    {
        public FirebaseClient FirebaseClient { get; set; }
        public IAuthService AuthService { get; set; }

        public FirebaseRepository(string firebaseUrl, IAuthService authService)
        {
            FirebaseClient = new FirebaseClient(
              firebaseUrl,
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => authService.GetFirebaseTokenAsync()
              });
            AuthService = authService;
        }
    }
}