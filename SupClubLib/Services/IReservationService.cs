using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupClubLib.Model;

namespace SupClubLib.Services
{
    public interface IReservationService
    {
        DateTime CurrentTime { get; set; }

        Task<ValidationResult> CancelReservationAsync(Reservation reservation);
        Task<ValidationResult> ConfirmReservationAsync(Reservation newReservation);
        Task<List<Board>> FindAvailableBoardsAsync(DateTime startDate, DateTime endDate);
        ValidationResult ValidateReservation(Reservation reservation);
    }
}