using Newtonsoft.Json;
using Plugin.SecureStorage;
using Plugin.SecureStorage.Abstractions;
using SupClubLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupClub.Helper
{
    public class SecureCredentials : ICredentials
    {
        private ISecureStorage _secureStorage;

        public SecureCredentials(ISecureStorage secureStorage = null)
        {
            if (secureStorage == null)
                _secureStorage = CrossSecureStorage.Current;
            else
                _secureStorage = secureStorage;
        }

        public string Password
        {
            get { return _secureStorage.GetValue("Password"); }
            set
            {
                _secureStorage.SetValue("Password", value);
            }
        }

        public ClubUser User
        {
            get
            {
                ClubUser user = null;
                string userJson = _secureStorage.GetValue("User");
                if (userJson != null)
                {
                    user = JsonConvert.DeserializeObject<ClubUser>(userJson);
                }
                return user;
            }
            set
            {
                string userJson = JsonConvert.SerializeObject(value);
                _secureStorage.SetValue("User", userJson);
            }
        }
    }
}
