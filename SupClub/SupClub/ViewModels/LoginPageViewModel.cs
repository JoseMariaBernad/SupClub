using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SupClub.Helper;
using SupClubLib.Model;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupClub.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        private readonly IAuthService _authService;
        private string _initialPage;
        private ClubUser _user;
        private string _password;

        private ICredentials _credentials;
        private string _email;

        public LoginPageViewModel(INavigationService navigationService,
                                  IPageDialogService pageDialogService,
                                  IAuthService authService,
                                  ICredentials credentials)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            _authService = authService;
            _credentials = credentials;
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (SetProperty(ref _email, value))
                    _user.Email = value;
            }
        }
        public string Password
        {
            get { return _password; }
            set {
                if (SetProperty(ref _password, value))
                {
                    _credentials.Password = _password;
                }
            }
        }

        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            InitializeVisualProperties();
            string initialPage = parameters.GetValue<string>("initialPage");
            if (initialPage != null)
            {
                _initialPage = initialPage;
            }
            await Login(_user, _password);
        }

        private void InitializeVisualProperties()
        {
            _user = _credentials.User;
            Password = _credentials.Password;
            if (_user != null)
            {
                Email = _user.Email;
            }
            else
            {
                _user = new ClubUser();
            }
        }

        public DelegateCommand LoginCommand => new DelegateCommand(
            async () => await Login(_user.Email, _password),
            () => _user != null && _password != null)
            .ObservesProperty(() => Email)
            .ObservesProperty(() => Password);

        private async Task Login(string email, string password)
        {
            if (email != null && password != null)
            {
                AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(email, password, true, false);
                if (string.Equals(result.Result, "Success"))
                {
                    _credentials.User = result.User;
                    await _navigationService.NavigateAsync(_initialPage);
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", result.Result, "OK");
                }
            }
        }

        private async Task Login(ClubUser user, string password)
        {
            if (user != null && password != null)
            {
                AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(user, password);
                if (string.Equals(result.Result, "Success"))
                {
                    await _navigationService.NavigateAsync(_initialPage);
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", result.Result, "OK");
                }
            }
        }

        public DelegateCommand RegisterCommand => new DelegateCommand(
            async () => await Register());

        private async Task Register()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("action", "Register");
            await _navigationService.NavigateAsync("UserProfilePage", parameters);
        }

        public DelegateCommand ResetPasswordCommand => new DelegateCommand(
            async () => await ResetPassword(),
            () => Email != null && Email != "")
            .ObservesProperty(() => Email);

        private async Task ResetPassword()
        {
            await _authService.ResetPassword(Email);
            await _pageDialogService.DisplayAlertAsync("Reset Password", 
                "Hemos enviado un mensaje a su correo electronico " 
                + Email + " con un enlace para cambiar la contraseña.", "OK");
        }
    }
}
