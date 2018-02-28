using Firebase.Database;
using Firebase.Database.Query;
using SupClubLib.Model;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLib.Repositories
{
    public class UserRepository : FirebaseRepository, IUserRepository
    {
        public UserRepository(string firebaseUrl, IAuthService authService) :
            base(firebaseUrl, authService) { }

        public async Task<ClubUser> SaveUserAsync(ClubUser user)
        {
            await FirebaseClient
                .Child($"users/{user.UserId}")
                .PutAsync(user);
            return user;
        }

        public async Task<ClubUser> FindUserByIdAsync(string userId)
        {
            return await FirebaseClient
                .Child($"users/{userId}")
                .OnceSingleAsync<ClubUser>();
        }
    }
}
