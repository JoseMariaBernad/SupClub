using NUnit.Framework;
using SupClubLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLibTest.Model
{
    [TestFixture]
    public class BoardTest
    {
        private Board _board;
        private List<Board> _boards;
        private List<Reservation> _reservations;

        [SetUp]
        public void PrepareTest()
        {
            _board = new Board()
            {
                BoardId = "TestBoardId1",
                Model = "Mistral 12,6 M2 Rígida Tipo RACE"
            };
            _boards = new List<Board> { _board };
            _reservations = new List<Reservation>();
        }

        [Category("FR_3")]
        [Test]
        public void FindAvailableBoards_Returns_All_Boards_When_No_ReservationExists()
        {
            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            Assert.That(boards.Count, Is.EqualTo(_boards.Count));
            CollectionAssert.AreEquivalent(_boards, boards);
        }

        [Category("FR_3")]
        [Test]
        public void FindAvailableBoards_Does_Not_Contain_Board_When_Board_Is_Reserved_In_Requested_Date_Range()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 24),
                    EndDate = new DateTime(2017, 11, 26),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Confirmed
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.DoesNotContain(boards, _board);
        }

        [Category("FR_3")]
        [Test]
        public void FindAvailableBoards_Returns_Board_When_Board_Is_Reserved_In_DifferentTimeRange()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 27),
                    EndDate = new DateTime(2017, 11, 29),
                    BoardId = _board.BoardId
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.Contains(boards, _board);
        }

        [Category("FR_3")]
        [Category("FR_3.1")]
        [Test]
        public void FindAvailableBoards_Does_Not_Contain_Board_When_Reserved_On_Day_Starting_On_The_Same_Day_The_Period_Finishes()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 26),
                    EndDate = new DateTime(2017, 11, 28),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Confirmed
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.DoesNotContain(boards, _board);
        }

        [Category("FR_3")]
        [Category("FR_3.1")]
        [Test]
        public void FindAvailableBoards_Does_Not_Contain_Board_When_Reserved_On_Day_Ending_On_The_Same_Day_The_Period_Starts()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 22),
                    EndDate = new DateTime(2017, 11, 24),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Confirmed
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.DoesNotContain(boards, _board);
        }

        [Category("FR_3")]
        [Test]
        public void FindAvailableBoards_Does_Not_Contain_Board_When_Reserved_On_Day_Ending_In_The_Middle_Of_Period()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 23),
                    EndDate = new DateTime(2017, 11, 25),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Confirmed
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.DoesNotContain(boards, _board);
        }

        [Category("FR_3")]
        [Test]
        public void FindAvailableBoards_Does_Not_Contain_Board_When_Reserved_On_Day_Starting_In_The_Middle_Of_Period()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 25),
                    EndDate = new DateTime(2017, 11, 27),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Confirmed
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.DoesNotContain(boards, _board);
        }

        [Category("FR_3")]
        [Category("FR_5.2")]
        [Test]
        public void FindAvailableBoards_Contains_Board_When_OverlapsTimePeriod_But_Reservation_Not_Confirmed()
        {
            Reservation reservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 25),
                    EndDate = new DateTime(2017, 11, 27),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Cancelled
                };
            _reservations.Add(reservation);

            List<Board> boards = Board.FindAvailableBoards(
                new DateTime(2017, 11, 24), new DateTime(2017, 11, 26), _boards, _reservations);

            CollectionAssert.Contains(boards, _board);
        }
    }
}
