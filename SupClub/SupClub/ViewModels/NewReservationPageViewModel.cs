using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SupClubLib.Model;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SupClub.ViewModels
{
	public class NewReservationPageViewModel : ViewModelBase
	{
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        private readonly IReservationService _reservationService;
        private readonly IAuthService _authService;

        private Reservation _reservation;
        public Reservation Reservation {
            get { return _reservation; }
            set { SetProperty(ref _reservation, value); }
        }

        private ObservableCollection<Board> _availableBoards;
        public ObservableCollection<Board> AvailableBoards
        {
            get { return _availableBoards; }
            set { SetProperty(ref _availableBoards, value); }
        }

        private Board _selectedBoard;
        public Board SelectedBoard
        {
            get => _selectedBoard;
            set
            {
                if (SetProperty(ref _selectedBoard, value) && value != null)
                {
                    Reservation.BoardId = value.BoardId;
                    Reservation.BoardModel = value.Model;
                }
            }   
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public NewReservationPageViewModel(INavigationService navigationService,
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
            Title = "Nueva Reserva";
            Reservation = new Reservation
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            Reservation.UserId = _authService.CurrentUser.UserId;
        }

        public DelegateCommand ShowAvailableBoardsCommand => new DelegateCommand(
            async ()=> await FindAvailableBoardsAsync(), 
            () => !IsBusy).ObservesProperty(() => IsBusy);

        private async Task FindAvailableBoardsAsync()
        {
            IsBusy = true;
            SelectedBoard = null;
            List<Board> boards = await _reservationService.FindAvailableBoardsAsync(Reservation.StartDate, Reservation.EndDate);
            AvailableBoards = new ObservableCollection<Board>(boards);
            IsBusy = false;
        }

        public DelegateCommand ReserveBoardCommand => new DelegateCommand(
            async () => await CreateReservationAsync(),
            () => SelectedBoard != null).ObservesProperty(() => SelectedBoard);

        private async Task CreateReservationAsync()
        {
            IsBusy = true;
            ValidationResult result = await _reservationService.ConfirmReservationAsync(Reservation);
            IsBusy = false;

            if (!result.IsValid)
            {
                await _pageDialogService.DisplayAlertAsync("Error creando reserva", result.Errors.FirstOrDefault(), "OK");
            }
            else
            {
                await _pageDialogService.DisplayAlertAsync("Crear reserva", "La reserva ha sido creada", "OK");
                await _navigationService.GoBackAsync();
            }
        }
    }
}
