using System.Threading.Tasks;
using SupClubLib.Model;

namespace SupClubLib.Repositories
{
    public interface IUserRepository
    {
        Task<ClubUser> FindUserByIdAsync(string userId);
        Task<ClubUser> SaveUserAsync(ClubUser user);
    }
}