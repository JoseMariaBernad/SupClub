using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLib.Model
{
    public class Board
    {
        public string BoardId { get; set; }
        public TableType Type { get; set; }
        public string Model { get; set; }
        public TableMode Mode { get; set; }
        public byte MinWeight { get; set; }
        public byte MaxWeight { get; set; }
        public Level Level { get; set; }
        public string UserId { get; set; }

        public static List<Board> FindAvailableBoards(DateTime startDate, DateTime endDate, List<Board> boards, List<Reservation> reservations)
        {
            List<Board> unavailableBoards = new List<Board>(boards.Count);

            foreach (Board board in boards)
            {
                if (board.IsUnavailableAtTimeFrame(startDate, endDate, reservations))
                {
                    unavailableBoards.Add(board);
                }
            }
            boards.RemoveAll(b => unavailableBoards.Contains(b));

            return boards;
        }

        public bool IsUnavailableAtTimeFrame(DateTime startDate, DateTime endDate, List<Reservation> reservations)
        {
            return BoardIsUnavailableAtTimeFrame(startDate, endDate, reservations, this);
        }

        private static bool BoardIsUnavailableAtTimeFrame(DateTime startDate, DateTime endDate, 
                                                            List<Reservation> reservations, Board board)
        {
            var matchingReservation = reservations.Find(
                reservation => reservation.Status == ReservationStatus.Confirmed
                                && string.Equals(reservation.BoardId, board.BoardId)
                                && reservation.OverlapsTimeFrame(startDate, endDate));

            return matchingReservation != null;
        }
    }

    public enum TableMode
    {
        Race = 1,
        AllAround = 2,
        Surf = 3,
        Touring = 4,
        Yoga = 5,
        Rivers = 6,
        MultiPerson = 7
    }

    public enum TableType
    {
        Inflatable = 1,
        Rigid = 2
    }
}
