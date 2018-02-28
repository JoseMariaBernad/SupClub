using Prism;
using Prism.Ioc;
using SupClub.ViewModels;
using SupClub.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.DryIoc;
using SupClubLib.Services;
using SupClubLib.Repositories;
using SupClubLib.Model;
using System;
using Prism.Navigation;
using SupClub.Helper;
using Plugin.SecureStorage;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SupClub
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("initialPage", "/NavigationPage/ReservationsPage");
            await NavigationService.NavigateAsync("NavigationPage/LoginPage", parameters);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            IAuthService authService = new AuthService("<YourFirebaseApiKey>");
            IUserRepository userRepository = new UserRepository("<YourFirebaseDatabaseURL>", authService);
            authService.UserRepository = userRepository;
            IReservationRepository reservationRepository = new ReservationRepository("<YourFirebaseDatabaseURL>", authService);
            IReservationService reservationService = new ReservationService(reservationRepository);

            //ICredentials credentials = new DummyCredentials();
            //credentials.Password = "testPassword";
            //credentials.User = new ClubUser { Email = "josemari.bernad@gmail.com" };
            ICredentials credentials = new SecureCredentials(CrossSecureStorage.Current);


            containerRegistry.RegisterInstance<IAuthService>(authService);
            containerRegistry.RegisterInstance<IReservationRepository>(reservationRepository);
            containerRegistry.RegisterInstance<IReservationService>(reservationService);
            containerRegistry.RegisterInstance<ICredentials>(credentials);

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<NewReservationPage>();
            containerRegistry.RegisterForNavigation<ReservationsPage>();
            containerRegistry.RegisterForNavigation<ReservationPage>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<UserProfilePage>();
        }
    }
}
