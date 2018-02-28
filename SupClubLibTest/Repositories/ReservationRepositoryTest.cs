using Firebase.Database;
using NUnit.Framework;
using SupClubLib.Model;
using SupClubLib.Repositories;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLibTest.Repositories
{
    [TestFixture]
    [Ignore("Integration tests")]
    public class ReservationRepositoryTest
    {
        private ClubUser _user;
        private ClubUser _validatedUser;
        private Board _board;
        private AuthService _authService;
        private ReservationRepository _sut;
        private Reservation _reservation;

        [SetUp]
        public void PrepareTest()
        {
            _user = new ClubUser
            {
                Name = "Pepe",
                Surname = "Bernad",
                Email = "pepe.bernad@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 12),
                Level = Level.Rookie,
                Phone = "976223344",
                Weight = 80
            };

            _validatedUser = new ClubUser
            {
                Name = "Jose Maria",
                Surname = "Bernad",
                Email = "josemari.bernad@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 12),
                Level = Level.Intermediate,
                Phone = "976223344",
                Weight = 80
            };

            _board = new Board
            {
                Level = Level.Intermediate,
                MaxWeight = 120,
                MinWeight = 50,
                Mode = TableMode.Race,
                Type = TableType.Rigid,
                Model = "Mistral 12,6 M2 Rígida Tipo RACE"
            };
            _reservation = new Reservation
            {
                Status = ReservationStatus.InProcess,
                StartDate = new DateTime(2017, 12, 15),
                EndDate = new DateTime(2017, 12, 18),
            };
            _authService = new AuthService("<YourFirebaseApiKey>");
            _sut = new ReservationRepository("<YourFirebaseDatabaseURL>", _authService);
        }

        [Test]
        [Ignore("One time test")]
        public async Task SaveBoard_Returns_New_Board_And_New_Board_Exists_In_Database()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            Board board = await _sut.SaveBoardAsync(_board);
            Assert.IsNotNull(board.BoardId);

            string id = board.BoardId;
            board = await _sut.FindBoardById(id);
            Assert.IsNotNull(board.BoardId);
        }
        // TODO: test SaveBoard with existing Board
        [Test]
        [Ignore("One time test")]
        public async Task SaveBoard_Throws_Error_When_Non_Admin_Tries_To_Add_Board()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_user, "testPassword", false);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            Assert.ThrowsAsync<FirebaseException>(() => _sut.SaveBoardAsync(_board));
        }

        [Test]
        //[Ignore("One time test")]
        public async Task SaveReservation_Returns_Reservation_And_Reservation_Exists_In_Database()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            _reservation.UserId = result.User.UserId;
            Reservation reservation = await _sut.SaveReservationAsync(_reservation);
            Assert.IsNotNull(reservation.ReservationId);

            string id = reservation.ReservationId;
            reservation = await _sut.FindReservationById(id);
            Assert.IsNotNull(reservation.ReservationId);
        }

        [Test]
        //[Ignore("One time test")]
        public async Task SaveReservation_Throws_Exception_When_Trying_To_Write_Reservation_For_Another_User()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            _reservation.UserId = "NotMyUserID";
            Assert.ThrowsAsync<FirebaseException>(() => _sut.SaveReservationAsync(_reservation));
        }
        // TODO: test SaveReservation with existing Reservation

        [Test]
        public async Task FindAllBoards_Returns_More_Than_0_Boards_And_BoardId_Is_Not_Null() {
            await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Board board = await _sut.SaveBoardAsync(_board);

            List<Board> boards = await _sut.FindAllBoardsAsync();
            Assert.Greater(boards.Count, 0);
            Assert.NotNull(boards.First().BoardId);
        }

        [Test]
        public async Task FindAllReservations_Returns_More_Than_0_Reservations()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            _reservation.UserId = result.User.UserId;
            Reservation reservation = await _sut.SaveReservationAsync(_reservation);
            Assert.IsNotNull(reservation.ReservationId);

            List<Reservation> reservations = await _sut.FindAllReservationsAsync();
            Assert.Greater(reservations.Count, 0);
            Assert.NotNull(reservations.First().ReservationId);
        }
    }
}
