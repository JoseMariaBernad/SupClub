using System;
using System.Collections.Generic;
using System.Text;
using SupClubLib.Model;

namespace SupClub.Helper
{
    public class DummyCredentials : ICredentials
    {
        public string Password { get; set ; }
        public ClubUser User { get ; set ; }
    }
}
