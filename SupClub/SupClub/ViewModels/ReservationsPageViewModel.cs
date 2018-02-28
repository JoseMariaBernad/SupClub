using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SupClubLib.Model;
using SupClubLib.Repositories;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SupClub.ViewModels
{
    public class ReservationsPageViewModel : ViewModelBase
    {
        public ReservationsPageViewModel(INavigationService navigationService,
                                            IAuthService authService,
                                            IReservationRepository reservationRepository)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _authService = authService;
            _reservationRepository = reservationRepository;
        }

        List<Reservation> _allReservations;
        private bool _onlyMine = true;
        private bool _onlyActive = true;
        private ObservableCollection<Reservation> _reservations;
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly IReservationRepository _reservationRepository;

        public bool OnlyMine
        {
            get { return _onlyMine; }
            set
            {
                if(SetProperty(ref _onlyMine, value))
                {
                    ApplyFiltersAndSort();
                }
            }
        }

        public bool OnlyActive
        {
            get { return _onlyActive; }
            set
            {
                if(SetProperty(ref _onlyActive, value))
                {
                    ApplyFiltersAndSort();
                }
            }
        }

        public ObservableCollection<Reservation> Reservations
        {
            get { return _reservations; }
            set
            {
                SetProperty(ref _reservations, value);
            }
        }

        private Reservation _selectedReservation;
        public Reservation SelectedReservation
        {
            get { return _selectedReservation; }
            set
            {
                SetProperty(ref _selectedReservation, value);
            }
        }

        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            Title = "Reservas";
            _allReservations = await _reservationRepository.FindAllReservationsAsync();
            ApplyFiltersAndSort();
        }

        private void ApplyFiltersAndSort()
        {
            List<Reservation> filteredReservations = _allReservations;
            if (_onlyMine)
            {
                filteredReservations = filteredReservations
                    .Where(r => r.UserId == _authService.CurrentUser.UserId)
                    .ToList();
            }
            if (_onlyActive)
            {
                filteredReservations = filteredReservations
                    .Where(r => r.EndDate >= DateTime.Today && r.Status != ReservationStatus.Cancelled)
                    .ToList();
            }
            filteredReservations = filteredReservations.OrderBy(r => r.StartDate).ToList();
            Reservations = new ObservableCollection<Reservation>(filteredReservations);
        }

        public DelegateCommand SelectCommand => new DelegateCommand(
                                                async () => await ShowReservation());

        private async Task ShowReservation()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("reservation", SelectedReservation);
            await _navigationService.NavigateAsync("ReservationPage", parameters);
        }

        public DelegateCommand NewReservationCommand => new DelegateCommand(
            async () => await NewReservation());

        private async Task NewReservation()
        {
            await _navigationService.NavigateAsync("NewReservationPage");
        }
    }
}
