using NSubstitute;
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;
using SupClub.Helper;
using SupClub.ViewModels;
using SupClubLib.Model;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubTest.ViewModels
{
    [TestFixture]
    public class LoginPageViewModelTest
    {
        INavigationService _navigationService;
        IPageDialogService _pageDialogService;
        ICredentials _secureCredentials;
        IAuthService _authService;
        NavigationParameters _parameters;
        LoginPageViewModel _sut;

        [SetUp]
        public void PrepareTest()
        {
            _navigationService = Substitute.For<INavigationService>();
            _pageDialogService = Substitute.For<IPageDialogService>();
            _secureCredentials = Substitute.For<ICredentials>();
            _authService = Substitute.For<IAuthService>();
            _parameters = new NavigationParameters();
            _parameters.Add("initialPage", "AnotherPage");
            _sut = new LoginPageViewModel(_navigationService, _pageDialogService, _authService, _secureCredentials);
        }

        [Test]
        public void OnNavigatedTo_Navigates_To_AnotherPage_When_Login_Is_Successful()
        {
            _secureCredentials.Password.Returns("testPassword");
            _secureCredentials.User.Returns(new SupClubLib.Model.ClubUser { Email = "test@email.org" });            
            _authService.LoginWithEmailAndPasswordAsync((ClubUser)null, null).ReturnsForAnyArgs(new AuthResult { Result = "Success" });

            _sut.OnNavigatedTo(_parameters);

            _navigationService.Received(1).NavigateAsync("AnotherPage");
        }

        [Test]
        public void OnNavigatedTo_Does_Not_Navigate_To_AnotherPage_When_Credentials_Are_Not_Entered()
        {
            _secureCredentials.Password.Returns((string)null);
            _secureCredentials.User.Returns((ClubUser)null);

            _sut.OnNavigatedTo(_parameters);

            _navigationService.Received(0).NavigateAsync("AnotherPage");
            Assert.IsFalse(_sut.LoginCommand.CanExecute());
        }

        [Test]
        public void LoginCommandTo_Navigates_To_AnotherPage_When_Login_Is_Successful_And_Saves_Credentials()
        {
            ClubUser testUser = new ClubUser
            {
                UserId = "TestID",
                Email = "testEmail",
                IsEmailVerified = true,
                Name = "Test Name",
                Surname = "Test Surname",
                Phone = "Test Phone"
            };
            ICredentials secureCredentials = new DummyCredentials();
            _authService.LoginWithEmailAndPasswordAsync("testEmail", "testPassword", true, false).Returns(
                new AuthResult
                {
                    Result = "Success",
                    User = testUser
                });
            _sut = new LoginPageViewModel(_navigationService, _pageDialogService, _authService, secureCredentials);
            _sut.OnNavigatedTo(_parameters);

            _sut.Email = "testEmail";
            _sut.Password = "testPassword";
            _sut.LoginCommand.Execute();

            Assert.That(secureCredentials.Password, Is.EqualTo("testPassword"));
            Assert.That(secureCredentials.User, Is.EqualTo(testUser));
            _navigationService.Received(1).NavigateAsync("AnotherPage");
        }
    }
}
