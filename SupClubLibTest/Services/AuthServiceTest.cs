using NSubstitute;
using NUnit.Framework;
using SupClubLib.Model;
using SupClubLib.Repositories;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLibTest.Services
{
    [TestFixture]
    //[Ignore("These are integration tests")]
    public class AuthServiceTest
    {
        ClubUser _user;
        ClubUser _validatedUser;
        IAuthService _sut;

        [SetUp]
        public void PrepareTest()
        {
            _user = new ClubUser
            {
                Name = "Pepe",
                Surname = "Bernad",
                Email = "pepe.bernad@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 12),
                Level = Level.Rookie,
                Phone = "976223344",
                Weight = 80
            };
            _validatedUser = new ClubUser
            {
                UserId = "TestID",
                Name = "Jose Maria",
                Surname = "Bernad",
                Email = "josemari.bernad@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 12),
                Level = Level.Intermediate,
                Phone = "976223344",
                Weight = 80
            };
            _sut = new AuthService("<YourFirebaseApiKey>");
            _sut.UserRepository = Substitute.For<IUserRepository>();
            _sut.UserRepository.SaveUserAsync(_validatedUser).ReturnsForAnyArgs(_validatedUser);
        }

        [Test]
        [Ignore("One Time test")]
        public async Task CreateUserAsync_Returns_User_WithNewID()
        {
            AuthResult result = await _sut.CreateUserAsync(_user, "testPassword", false);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result);
            Assert.IsFalse(result.User.IsEmailVerified);
        }

        [Test]
        [Ignore("One Time test")]
        public async Task CreateUserAsync_Returns_Mail_Exists_Reason_And_Null_UserId_WithExistingEmail()
        {
            AuthResult result = await _sut.CreateUserAsync(_user, "testPassword", false);
            result = await _sut.CreateUserAsync(_user, "testPassword", false);
            Assert.IsNotNull(result.User);
            Assert.IsNull(result.User.UserId);
            Assert.AreEqual("Error: Mail Exists", result.Result);
            Assert.IsFalse(result.User.IsEmailVerified);
        }

        [Test]
        [Ignore("One Time test")]
        public async Task CreateUserAsync_Returns_User_WithNewID_FirstTimeValidUser()
        {
            AuthResult result = await _sut.CreateUserAsync(_validatedUser, "testPassword", true);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result);
            Assert.IsFalse(result.User.IsEmailVerified);
        }

        [Test]
        public async Task LoginWithEmailAndPasswordAsync_Returns_User_With_ID_When_UserEmail_Is_Validated()
        {
            AuthResult result = await _sut.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result);
            Assert.IsTrue(result.User.IsEmailVerified);
            await _sut.UserRepository.Received(1).SaveUserAsync(_validatedUser);
            Assert.AreEqual(result.User, _sut.CurrentUser);
        }

        [Test]
        public async Task LoginWithEmailAndPasswordAsync_Returns_User_With_Id_And_Error_Result_EnforceValidationTrue()
        {
            AuthResult result = await _sut.LoginWithEmailAndPasswordAsync(_user, "testPassword", true);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Error: Please, validate your email.", result.Result);
            Assert.IsFalse(result.User.IsEmailVerified);
        }

        [Test]
        public async Task LoginWithEmailAndPasswordAsync_Returns_User_With_Id_And_Success_Result_EnforceValidationFalse()
        {
            AuthResult result = await _sut.LoginWithEmailAndPasswordAsync(_user, "testPassword", false);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result);
            Assert.IsFalse(result.User.IsEmailVerified);
        }

        [Test]
        public async Task LoginWithEmailAndPasswordAsync_Returns_User_With_NullId_And_Error_Result_InvalidCredentials()
        {
            AuthResult result = await _sut.LoginWithEmailAndPasswordAsync(_user, "wrongPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNull(result.User.UserId);
            Assert.AreEqual("Error: Invalid credentials", result.Result);
            Assert.IsFalse(result.User.IsEmailVerified);
        }

        //TODO: Test weak password (< 6 characters)

        // TODO: Test this method with the other 3 posibilities of enforceValidation and saveUser
        //                                                          true                    true
        //                                                          false                   false
        //                                                          false                   true
        [Test]
        public async Task LoginWithEmailAndPasswordAsync_Overload_AsString_Returns_Existing_User_From_Service()
        {
            _sut.UserRepository.FindUserByIdAsync("TestID").Returns(_validatedUser);

            AuthResult result = await _sut.LoginWithEmailAndPasswordAsync(_validatedUser.Email, "testPassword", true, false);

            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result);
            Assert.IsTrue(result.User.IsEmailVerified);
            await _sut.UserRepository.Received(1).FindUserByIdAsync(result.User.UserId);
            await _sut.UserRepository.Received(0).SaveUserAsync(_validatedUser);
            Assert.AreEqual(_validatedUser.Email, result.User.Email);
        }
    }
}
