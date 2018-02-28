using SupClubLib.Model;

namespace SupClub.Helper
{
    public interface ICredentials
    {
        string Password { get; set; }
        ClubUser User { get; set; }
    }
}