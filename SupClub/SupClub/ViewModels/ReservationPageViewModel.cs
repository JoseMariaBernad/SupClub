using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SupClubLib.Model;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupClub.ViewModels
{
    public class ReservationPageViewModel : ViewModelBase
    {
        private Reservation _reservation;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        private readonly IReservationService _reservationService;
        private readonly IAuthService _authService;

        public Reservation Reservation
        {
            get { return _reservation; }
            set
            {
                SetProperty(ref _reservation, value);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ReservationPageViewModel(INavigationService navigationService,
                                        IPageDialogService pageDialogService,
                                        IReservationService reservationService,
                                        IAuthService authService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            _reservationService = reservationService;
            _authService = authService;
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            IsBusy = true;
            Reservation = parameters.GetValue<Reservation>("reservation");
            ShowTitle();
            IsBusy = false;
        }

        private void ShowTitle()
        {
            string status;

            switch (Reservation.Status)
            {
                case ReservationStatus.InProcess:
                    status = "En proceso";
                    break;
                case ReservationStatus.Confirmed:
                    status = "Confirmada";
                    break;
                case ReservationStatus.Cancelled:
                    status = "Cancelada";
                    break;
                default:
                    status = "Reserva sin estado";
                    break;
            }

            Title = "Reserva " + status;
        }

        public DelegateCommand CancelReservationCommand => new DelegateCommand(
            async () => await CancelReservationAsync(),
            () => CanCancelReservation())
            .ObservesProperty(() => IsBusy);

        private bool CanCancelReservation()
        {
                return !IsBusy && Reservation != null && Reservation.Status == ReservationStatus.Confirmed
                        && Reservation.UserId == _authService.CurrentUser.UserId;
        }

        private async Task CancelReservationAsync()
        {
            bool confirmation = await _pageDialogService.DisplayAlertAsync("Cancelar reserva",
                "Está seguro que desea cancelar la reserva?",
                "Quiero cancelar la reserva",
                "No quiero cancelar la reserva");
            if (confirmation)
            {
                IsBusy = true;
                ValidationResult result = await _reservationService.CancelReservationAsync(Reservation);
                IsBusy = false;

                if (!result.IsValid)
                {
                    await _pageDialogService.DisplayAlertAsync("Error cancelando reserva", result.Errors.FirstOrDefault(), "OK");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Cancelar reserva", "La reserva ha sido cancelada", "OK");
                    //await _navigationService.GoBackAsync();
                }
            }
            ShowTitle();
        }
    }
}
