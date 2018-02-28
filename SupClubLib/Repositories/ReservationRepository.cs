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
    public class ReservationRepository : FirebaseRepository, IReservationRepository
    {
        public ReservationRepository(string firebaseUrl, IAuthService authService)
            : base(firebaseUrl, authService) { }

        public async Task<Board> SaveBoardAsync(Board board)
        {
            if (board.BoardId != null)
            {
                await FirebaseClient
                    .Child($"boards/{board.BoardId}")
                    .PutAsync(board);
            }
            else
            {
                var newBoard = await FirebaseClient
                    .Child($"boards")
                    .PostAsync(board);
                board.BoardId = newBoard.Key;
            }

            return board;
        }

        public async Task<Board> FindBoardById(string boardId)
        {
            var board = await FirebaseClient
                .Child($"boards/{boardId}")
                .OnceSingleAsync<Board>();
            board.BoardId = boardId;
            return board;
        }

        public async Task<List<Board>> FindAllBoardsAsync()
        {
            var tmpBoards = await FirebaseClient
                .Child("boards")
                .OnceAsync<Board>();
            List<Board> boards = new List<Board>(tmpBoards.Count);
            foreach (var board in tmpBoards)
            {
                board.Object.BoardId = board.Key;
                boards.Add(board.Object);
            }

            return boards;
        }

        public async Task<Reservation> SaveReservationAsync(Reservation reservation)
        {
            if (reservation.ReservationId != null)
            {
                await FirebaseClient
                    .Child($"reservations/{reservation.ReservationId}")
                    .PutAsync(reservation);
            }
            else
            {
                var newReservation = await FirebaseClient
                    .Child($"reservations")
                    .PostAsync(reservation);
                reservation.ReservationId = newReservation.Key;
            }

            return reservation;
        }

        public async Task<Reservation> FindReservationById(string reservationId)
        {
            var reservation = await FirebaseClient
                .Child($"reservations/{reservationId}")
                .OnceSingleAsync<Reservation>();
            reservation.ReservationId = reservationId;
            return reservation;
        }

        public async Task<List<Reservation>> FindAllReservationsAsync()
        {
            var tmpReservations = await FirebaseClient
                .Child("reservations")
                .OnceAsync<Reservation>();
            List<Reservation> reservations = new List<Reservation>(tmpReservations.Count);
            foreach (var reservation in tmpReservations)
            {
                reservation.Object.ReservationId = reservation.Key;
                reservations.Add(reservation.Object);
            }

            return reservations;
        }
    }
}
