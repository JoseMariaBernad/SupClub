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
    public class ReservationTest
    {
        private Board _board;

        [SetUp]
        public void PrepareTest()
        {
            _board = new Board()
            {
                BoardId = "TestBoardId1",
                Model = "Mistral 12,6 M2 Rígida Tipo RACE"
            };
        }

        [Category("FR_1")]
        [Test]
        public void Validate_Returns_Error_When_Reservation_Starts_35_Days_In_The_Future()
        {
            DateTime currentTime = new DateTime(2017, 10, 20);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 24),
                    EndDate = new DateTime(2017, 11, 26),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.Contains(result.Errors, "No se puede reservar con mas de 30 días de antelación. Cambie la fecha de inicio.");
            Assert.That(result.IsValid, Is.False);
        }

        [Category("FR_1")]
        [Test]
        public void Validate_Does_Not_Return_Error_When_Reservation_Starts_10_Days_In_The_Future()
        {
            DateTime currentTime = new DateTime(2017, 11, 20);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 30),
                    EndDate = new DateTime(2017, 12, 2),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "No se puede reservar con mas de 30 días de antelación. Cambie la fecha de inicio.");
            CollectionAssert.DoesNotContain(result.Errors, "La fecha de inicio no puede ser anterior a hoy.");
            Assert.That(result.IsValid, Is.True);
        }

        [Category("FR_1")]
        [Category("FR_1.1")]
        [Test]
        public void Validate_Returns_Error_When_Reservation_Starts_In_The_Past()
        {
            DateTime currentTime = new DateTime(2017, 11, 25);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 24),
                    EndDate = new DateTime(2017, 11, 26),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.Contains(result.Errors, "La fecha de inicio no puede ser anterior a hoy.");
            Assert.That(result.IsValid, Is.False);
        }

        [Category("FR_1")]
        [Category("FR_1.2")]
        [Test]
        public void Validate_Does_Not_Return_Error_When_Reservation_Starts_At_Current_Date()
        {
            DateTime currentTime = new DateTime(2017, 11, 25);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 30),
                    EndDate = new DateTime(2017, 12, 2),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "No se puede reservar con mas de 30 días de antelación. Cambie la fecha de inicio.");
            CollectionAssert.DoesNotContain(result.Errors, "La fecha de inicio no puede ser anterior a hoy.");
            Assert.That(result.IsValid, Is.True);
        }

        [Category("FR_2")]
        [Test]
        public void Validate_Does_Not_Return_Error_When_Total_Days_Of_Reservation_Are_3()
        {
            DateTime currentTime = new DateTime(2017, 11, 20);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 24),
                    EndDate = new DateTime(2017, 11, 26),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "Debe reservar al menos 3 días.");
            Assert.That(result.IsValid, Is.True);
        }

        [Category("FR_2")]
        [Test]
        public void Validate_Does_Not_Return_Error_When_Total_Days_Of_Reservation_Are_2()
        {
            DateTime currentTime = new DateTime(2017, 11, 20);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 24),
                    EndDate = new DateTime(2017, 11, 25),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "Debe reservar al menos 3 días.");
            CollectionAssert.DoesNotContain(result.Errors, "Debe reservar al menos 1 día.");
            Assert.That(result.IsValid, Is.True);
        }

        [Category("FR_2")]
        [Test]
        public void Validate_Does_Not_Return_Error_When_Total_Days_Of_Reservation_Is_1()
        {
            DateTime currentTime = new DateTime(2017, 11, 20);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 11, 24),
                    EndDate = new DateTime(2017, 11, 24),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "Debe reservar al menos 3 días.");
            CollectionAssert.DoesNotContain(result.Errors, "Debe reservar al menos 1 día.");
            Assert.That(result.IsValid, Is.True);
        }

        [Category("FR_2")]
        [Category("FR_2.1")]
        [Test]
        public void Validate_Returns_Error_When_Reservation_Lasts_16_Days()
        {
            DateTime currentTime = new DateTime(2017, 12, 1);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 12, 1),
                    EndDate = new DateTime(2017, 12, 16),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.Contains(result.Errors, "La reserva sólo puede durar como máximo 15 días.");
            Assert.That(result.IsValid, Is.False);
        }

        [Category("FR_2")]
        [Category("FR_2.1")]
        [Test]
        public void Validate_Does_Not_Return_Error_When_Reservation_Lasts_15_Days()
        {
            DateTime currentTime = new DateTime(2017, 12, 1);
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 12, 1),
                    EndDate = new DateTime(2017, 12, 15),
                    BoardId = _board.BoardId
                };

            ValidationResult result = sut.Validate(currentTime);

            Assert.That(result, Is.Not.Null);
            CollectionAssert.DoesNotContain(result.Errors, "La reserva sólo puede durar como máximo 15 días.");
            Assert.That(result.IsValid, Is.True);
        }

        [Category("FR_4")]
        [Category("FR_5.1")]
        [Test]
        public void OverlapsReservation_Returns_False_When_Overlaps_Time_Frame_But_Existing_Reservation_Is_Not_Confirmed()
        {
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 12, 1),
                    EndDate = new DateTime(2017, 12, 15),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.InProcess
                };
            Reservation existingReservation = new Reservation()
            {
                StartDate = new DateTime(2017, 12, 2),
                EndDate = new DateTime(2017, 12, 15),
                BoardId = _board.BoardId,
                Status = ReservationStatus.Cancelled                
            };

            bool overlaps = sut.OverlapsReservation(existingReservation);
            
            Assert.That(overlaps, Is.False);
        }

        [Category("FR_4")]
        [Category("FR_5.1")]
        [Test]
        public void OverlapsReservation_Returns_True_When_Overlaps_Time_Frame_And_Current_Reservation_IsConfirmed()
        {
            Reservation sut =
                new Reservation()
                {
                    StartDate = new DateTime(2017, 12, 1),
                    EndDate = new DateTime(2017, 12, 15),
                    BoardId = _board.BoardId,
                    Status = ReservationStatus.Confirmed
                };
            Reservation existingReservation = new Reservation()
            {
                StartDate = new DateTime(2017, 12, 2),
                EndDate = new DateTime(2017, 12, 15),
                BoardId = _board.BoardId,
                Status = ReservationStatus.Cancelled
            };

            bool overlaps = sut.OverlapsReservation(existingReservation);

            Assert.That(overlaps, Is.True);
        }
    }
}
