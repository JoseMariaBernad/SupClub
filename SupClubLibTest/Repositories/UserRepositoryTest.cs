using Firebase.Database;
using NUnit.Framework;
using SupClubLib.Model;
using SupClubLib.Repositories;
using SupClubLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLibTest.Repositories
{
    [TestFixture]
    //[Ignore("IntegrationTests")]
    public class UserRepositoryTest
    {
        private ClubUser _validatedUser;
        private ClubUser _user;
        private AuthService _authService;
        private UserRepository _sut;

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
                Name = "Jose Maria",
                Surname = "Bernad",
                Email = "josemari.bernad@gmail.com",
                DateOfBirth = new DateTime(1990, 1, 12),
                Level = Level.Intermediate,
                Phone = "976223344",
                Weight = 80
            };
            _authService = new AuthService("<YourFirebaseApiKey>");
            _sut = new UserRepository("<YourFirebaseDatabaseURL>", _authService);
            _authService.UserRepository = _sut;
        }

        [Test]
        [Ignore("One time test")]
        public async Task AddUser_Returns_User_And_User_Exists_In_Database()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            ClubUser user = await _sut.SaveUserAsync(result.User);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);

            user = await _sut.FindUserByIdAsync(user.UserId);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
        }

        [Test]
        [Ignore("One time test")]
        public async Task AddUser_Throws_Exception_When_Email_Not_Validated()
        {
/* Test this rule
 * {
  "rules": {
    "users": {
      "$userId": {
        ".write": "auth.token.email_verified == true"
      }
    }
  }
}
*/
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_user, "testPassword", false);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.IsFalse(result.User.IsEmailVerified);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            Assert.ThrowsAsync<FirebaseException>(() => _sut.SaveUserAsync(result.User));
        }

        [Test]
        [Ignore("One time test")]
        public async Task AddUser_Throws_Exception_When_Trying_To_Write_Different_User_Than_Specified_Credentials()
        {
            /* Test this rule
             * {
              "rules": {
                "users": {
                  "$userId": {
                    ".write": "auth.uid == $userId"
                  }
                }
              }
            }
            */
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.IsTrue(result.User.IsEmailVerified);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            Assert.ThrowsAsync<FirebaseException>(() => _sut.SaveUserAsync(_user));
        }

        [Test]
        public async Task FindUserById_Throws_Exception_When_Trying_To_Read_Different_User_Than_Specified_Credentials()
        {
            /* Test this rule
             * {
              "rules": {
                "users": {
                  "$userId": {
                    ".read": "auth.uid == $userId"
                  }
                }
              }
            }
            */
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_validatedUser, "testPassword");
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.IsTrue(result.User.IsEmailVerified);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            Assert.ThrowsAsync<FirebaseException>(() => _sut.FindUserByIdAsync("dDYtlWFbcBfJaePAv2ZotoopccR2"));
        }

        [Test]
        public async Task FindUserById_Returns_Null_When_User_Does_Not_Exist()
        {
            AuthResult result = await _authService.LoginWithEmailAndPasswordAsync(_user, "testPassword", false, false);
            Assert.IsNotNull(result.User);
            Assert.IsNotNull(result.User.UserId);
            Assert.IsFalse(result.User.IsEmailVerified);
            Assert.AreEqual("Success", result.Result); //check that preparation is successful

            ClubUser user = await _sut.FindUserByIdAsync(result.User.UserId);

            Assert.That(user, Is.Null);
        }
    }
}
