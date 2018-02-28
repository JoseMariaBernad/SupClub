using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLib.Model
{
    public class Reservation
    {
        private const int MIN_DAYS = 1;
        private const int MAX_DAYS = 15;
        public string ReservationId { get; set; }
        public string UserId { get; set; }
        public string BoardId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReservationStatus Status { get; set; }
        public string BoardModel { get; set; }

        [JsonIgnore]
        public string TimeFrame
        {
            get
            {
                return StartDate.ToString("dd/MM/yyyy") + " - " + EndDate.ToString("dd/MM/yyyy");
            }
        }

        public static ValidationResult ValidateStartDate(Reservation reservation, DateTime currentTime)
        {
            ValidationResult result = new ValidationResult();
            result.IsValid = true;

            if (reservation.StartDate < currentTime)
            {
                result.Errors.Add("La fecha de inicio no puede ser anterior a hoy.");
                result.IsValid = false;
            }

            TimeSpan diff = reservation.StartDate.Subtract(currentTime);
            if (diff.Days > 30)
            {
                result.Errors.Add("No se puede reservar con mas de 30 días de antelación. Cambie la fecha de inicio.");
                result.IsValid = false;
            }

            return result;
        }

        private static ValidationResult ValidateEndDate(Reservation reservation, ValidationResult result = null)
        {
            if (result == null)
            {
                result = new ValidationResult();
            }

            TimeSpan diff = reservation.EndDate - reservation.StartDate;

            if (diff.Days < MIN_DAYS - 1)
            {
                result.Errors.Add($"Debe reservar al menos {MIN_DAYS} día.");
                result.IsValid = false;
            }

            if (diff.Days > MAX_DAYS - 1)
            {
                result.Errors.Add("La reserva sólo puede durar como máximo 15 días.");
                result.IsValid = false;
            }

            return result;
        }

        private static bool OverlapsTimeFrame(DateTime startDate, DateTime endDate, Reservation reservation)
        {
            return (reservation.StartDate <= endDate && startDate <= reservation.EndDate);
        }

        public bool OverlapsTimeFrame(DateTime startDate, DateTime endDate)
        {
            return OverlapsTimeFrame(startDate, endDate, this);
        }

        private static ValidationResult Validate(Reservation reservation, DateTime currentTime)
        {
            ValidationResult result = Reservation.ValidateStartDate(reservation, currentTime);
            result = ValidateEndDate(reservation, result);
            return result;
        }

        public ValidationResult Validate(DateTime currentTime)
        {
            return Validate(this, currentTime);
        }


        public bool OverlapsReservation(Reservation reservation)
        {
            return (reservation.Status == ReservationStatus.Confirmed
                    || this.Status == ReservationStatus.Confirmed)
                    && this.OverlapsTimeFrame(reservation.StartDate, reservation.EndDate)
                    && string.Equals(this.BoardId, reservation.BoardId);
        }

        private static ValidationResult ConfirmReservation(Reservation newReservation, List<Reservation> reservations, DateTime currentTime)
        {
            ValidationResult result = newReservation.Validate(currentTime);

            foreach (var reservation in reservations)
            {
                if (newReservation.OverlapsReservation(reservation))
                {
                    result.Errors.Add("La reserva tiene conflicto con otra reserva existente. Pruebe a seleccionar otra tabla.");
                    result.IsValid = false;
                }
            }

            if (result.IsValid)
            {
                newReservation.Status = ReservationStatus.Confirmed;
            }

            return result;
        }

        public ValidationResult Confirm(List<Reservation> existingReservations, DateTime currentTime)
        {
            return ConfirmReservation(this, existingReservations, currentTime);
        }

        public static ValidationResult CancelReservation(Reservation reservation, DateTime currentTime)
        {
            ValidationResult result = new ValidationResult();
            result.IsValid = true;

            if (reservation.StartDate <= currentTime)
            {
                result.Errors.Add("Solo se puede cancelar la reserva antes de que llegue el día de comienzo.");
                result.IsValid = false;
            }

            if (result.IsValid)
            {
                reservation.Status = ReservationStatus.Cancelled;
            }

            return result;
        }

        public ValidationResult Cancel(DateTime currentTime)
        {
            return CancelReservation(this, currentTime);
        }
    }

    public enum ReservationStatus
    {
        InProcess = 0,
        //Requested = 1,
        Confirmed = 2,
        Cancelled = 3
    }
}
