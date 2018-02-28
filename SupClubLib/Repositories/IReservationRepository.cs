using System.Collections.Generic;
using System.Threading.Tasks;
using SupClubLib.Model;

namespace SupClubLib.Repositories
{
    public interface IReservationRepository
    {
        Task<List<Board>> FindAllBoardsAsync();
        Task<List<Reservation>> FindAllReservationsAsync();
        Task<Board> FindBoardById(string boardId);
        Task<Reservation> FindReservationById(string reservationId);
        Task<Board> SaveBoardAsync(Board board);
        Task<Reservation> SaveReservationAsync(Reservation reservation);
    }
}