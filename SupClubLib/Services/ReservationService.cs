using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupClubLib.Model;
using SupClubLib.Repositories;
using System.ComponentModel;
using System.Collections;

namespace SupClubLib.Services
{
    public class ReservationService : IReservationService
    {
        private DateTime _currentTime;
        private IReservationRepository _repository;

        public ReservationService(IReservationRepository repository)
        {
            _repository = repository;
        }

        public DateTime CurrentTime
        {
            get
            {
                if (_currentTime == DateTime.MinValue)
                {
                    return DateTime.Today;
                }
                else
                {
                    return _currentTime;
                }
            }

            set
            {
                _currentTime = value;
            }
        }

        public async Task<List<Board>> FindAvailableBoardsAsync(DateTime startDate, DateTime endDate)
        {
            List<Board> candidateBoards = await _repository.FindAllBoardsAsync();
            List<Reservation> reservations = await _repository.FindAllReservationsAsync();

            return Board.FindAvailableBoards(startDate, endDate, candidateBoards, reservations);
        }

        public ValidationResult ValidateReservation(Reservation reservation)
        {
            ValidationResult result = reservation.Validate(CurrentTime);

            return result;
        }

        public async Task<ValidationResult> ConfirmReservationAsync(Reservation newReservation)
        {
            List<Reservation> reservations = await _repository.FindAllReservationsAsync();
            ValidationResult result = newReservation.Confirm(reservations, CurrentTime);

            if (result.IsValid)
            {
                await _repository.SaveReservationAsync(newReservation);
            }

            return result;
        }

        public async Task<ValidationResult> CancelReservationAsync(Reservation reservation)
        {
            ValidationResult result = reservation.Cancel(CurrentTime);

            if (result.IsValid)
            {
                await _repository.SaveReservationAsync(reservation);
            }

            return result;
        }
    }
}
