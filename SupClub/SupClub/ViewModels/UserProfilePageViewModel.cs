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
    public class UserProfilePageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IAuthService _authService;
        private readonly ICredentials _credentials;
        private string _action;
        private ClubUser _user;
        private string _password;

        public ClubUser User
        {
            get { return _user; }
            set
            {
                SetProperty(ref _user, value);
            }
        }

        public UserProfilePageViewModel(INavigationService navigationService,
                                        IPageDialogService pageDialogService,
                                        IAuthService authService,
                                        ICredentials credentials
                                  )
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _authService = authService;
            _credentials = credentials;
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            _action = parameters.GetValue<string>("action");
            Title = _action;
            ClubUser user = _credentials.User;
            if (user == null)
            {
                User = new ClubUser();
            }
            else
            {
                User = user;
            }
            string password = _credentials.Password;
            if (password != null)
            {
                Password = password;
            }
        }

        public DelegateCommand RegisterCommand => new DelegateCommand(
            async () => await Register(),
            () => User != null && User.Email != "" && Password != "")
            .ObservesProperty(() => Password);

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        private async Task Register()
        {
            ValidationResult result = User.Validate();
            if (result.IsValid)
            {
                AuthResult aResult = await _authService.CreateUserAsync(User, Password, true);
                ValidatePassword(aResult);
                if (aResult.Result == "Success")
                {
                    await _pageDialogService.DisplayAlertAsync("Usuario registrado", "Hemos enviado un email de validación a la dirección de correo que nos ha facilitado. Por favor, revise su correo y haga click en el enlace de validación para poder utilizar la aplicación.", "OK");
                    _credentials.User = User;
                    _credentials.Password = Password;
                    await NavigationService.GoBackAsync();
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error de registro: ", aResult.Result, "OK");
                }
            }
            else
            {
                await _pageDialogService.DisplayAlertAsync("Error", result.Errors.FirstOrDefault(), "OK");
            }
        }

        private void ValidatePassword(AuthResult result)
        {
            if (Password == null || Password.Length < 6)
            {
                result.Result = "La password debe tener al menos 6 caracteres";
            }
        }
    }
}
