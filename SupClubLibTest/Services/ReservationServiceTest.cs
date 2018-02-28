using NSubstitute;
using NUnit.Framework;
using SupClubLib.Model;
using SupClubLib.Repositories;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLibTest.Services
{
    [TestFixture]
    public class ReservationServiceTest
    {
        private Board _board;
        private List<Board> _boards;
        private List<Reservation> _reservations;
        private IReservationRepository _repository;
        private ReservationService _sut;

        [SetUp]
        public void PrepareTest()
        {
            _repository = Substitute.For<IReservationRepository>();
            _board = new Board() {
                BoardId = "TestBoardId1",
                Model = "Mistral 12,6 M2 Rígida Tipo RACE" };
            _boards = new List<Board> { _board };
            _reservations = new List<Reservation>();
            _repository.FindAllBoardsAsync().Returns(_boards);
            _repository.FindAllReservationsAsync().Returns(_reservations);
            _sut = new ReservationService(_repository);
        }


        [Category("FR_4")]
        [Category("FR_4.1")]
        [Test]
        public async Task ConfirmReservation_Returns_Error_When_Reservation_Overlaps_Another_Reservation()
        {
            Reservation newReservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 12, 1),
                    EndDate = new DateTime(2017, 12, 15),
                    BoardId = _board.BoardId
                };
            Reservation existingReservation = new Reservation()
            {
                StartDate = new DateTime(2017, 12, 2),
                EndDate = new DateTime(2017, 12, 15),
                BoardId = _board.BoardId,
                Status = ReservationStatus.Confirmed
            };
            _reservations.Add(existingReservation);

            ValidationResult result = await _sut.ConfirmReservationAsync(newReservation);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.Contains(result.Errors, "La reserva tiene conflicto con otra reserva existente. Pruebe a seleccionar otra tabla.");
            Assert.That(result.IsValid, Is.False);
        }

        [Category("FR_4")]
        [Category("FR_4.1")]
        [Test]
        public async Task ConfirmReservation_Does_Not_Return_Error_When_No_Other_Reservation_Exists()
        {
            _sut.CurrentTime = new DateTime(2017, 11, 30);
            Reservation newReservation =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 12, 1),
                    EndDate = new DateTime(2017, 12, 15),
                    BoardId = _board.BoardId
                };

            ValidationResult result = await _sut.ConfirmReservationAsync(newReservation);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "La reserva tiene conflicto con otra reserva existente. Pruebe a seleccionar otra tabla.");
            Assert.That(result.IsValid, Is.True);
            Assert.That(newReservation.Status, Is.EqualTo(ReservationStatus.Confirmed));
            await _repository.Received(1).SaveReservationAsync(newReservation);
        }

        [Category("FR_5")]
        [Test]
        public async Task CancelReservation_Returns_Error_When_CurrentDay_Equals_Reservation_StartDate()
        {
            _sut.CurrentTime = new DateTime(2017, 12, 2);
            Reservation existingReservation = new Reservation()
            {
                StartDate = new DateTime(2017, 12, 2),
                EndDate = new DateTime(2017, 12, 15),
                BoardId = _board.BoardId,
                Status = ReservationStatus.Confirmed
            };
            _reservations.Add(existingReservation);

            ValidationResult result = await _sut.CancelReservationAsync(existingReservation);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.Contains(result.Errors, "Solo se puede cancelar la reserva antes de que llegue el día de comienzo.");
            Assert.That(result.IsValid, Is.False);
            Assert.That(existingReservation.Status, Is.Not.EqualTo(ReservationStatus.Cancelled));
        }

        [Category("FR_5")]
        [Test]
        public async Task CancelReservation_Does_Not_Return_Error_When_CurrentDay_Before_Reservation_StartDate()
        {
            _sut.CurrentTime = new DateTime(2017, 12, 1);
            Reservation existingReservation = new Reservation()
            {
                StartDate = new DateTime(2017, 12, 2),
                EndDate = new DateTime(2017, 12, 15),
                BoardId = _board.BoardId,
                Status = ReservationStatus.Confirmed
            };
            _reservations.Add(existingReservation);

            ValidationResult result = await _sut.CancelReservationAsync(existingReservation);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "Solo se puede cancelar la reserva antes de que llegue el día de comienzo.");
            Assert.That(result.IsValid, Is.True);
            Assert.That(existingReservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
        }
    }
}
