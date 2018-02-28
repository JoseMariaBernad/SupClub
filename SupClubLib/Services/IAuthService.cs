using System.Threading.Tasks;
using SupClubLib.Model;
using SupClubLib.Repositories;

namespace SupClubLib.Services
{
    public interface IAuthService
    {
        IUserRepository UserRepository { get; set; }
        ClubUser CurrentUser { get; }

        Task<AuthResult> CreateUserAsync(ClubUser user, string password, bool sendVerificationEmail);
        Task<string> GetFirebaseTokenAsync();
        Task<AuthResult> LoginWithEmailAndPasswordAsync(ClubUser user, string password, bool enforceValidation = true, bool saveUser = true);
        Task ResetPassword(string email);
        Task<AuthResult> LoginWithEmailAndPasswordAsync(string email, string password, bool enforceValidation = true, bool saveUser = true);
    }
}