using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Plugin.SecureStorage.Abstractions;
using SupClub.Helper;
using SupClubLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupClubTest.Helper
{
    [TestFixture]
    public class SecureCredentialsTest
    {
        ISecureStorage _storage;
        SecureCredentials _sut;

        [SetUp]
        public void PrepareTests()
        {
            _storage = Substitute.For<ISecureStorage>();
            _sut = new SecureCredentials(_storage);
        }

        [Test]
        public void Values_Are_Null_When_Do_Not_Exist()
        {
            _storage.GetValue("Password").Returns((string)null);
            _storage.GetValue("User").Returns((string)null);

            Assert.Null(_sut.Password);
            Assert.Null(_sut.User);
        }

        [Test]
        public void Values_Are_Not_Null_When_Exist()
        {
            _storage.GetValue("Password").Returns("myTestPassword");
            ClubUser user = new ClubUser { Email = "me@myserver.es" };
            string userJson = JsonConvert.SerializeObject(user);
            _storage.GetValue("User").Returns(userJson);

            Assert.AreEqual("myTestPassword", _sut.Password);
            Assert.AreEqual(user.Email, _sut.User.Email);
        }
    }
}
