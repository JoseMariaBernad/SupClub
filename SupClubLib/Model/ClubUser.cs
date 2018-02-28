using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLib.Model
{
    public class ClubUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; } //Nice to have
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte Weight { get; set; } //Nice to have
        public Level Level { get; set; } //Nice to have
        public bool IsEmailVerified { get; set; }

        public ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult { IsValid = true };
            if(Name == null
                || Surname == null
                || Email == null
                || Phone == null)
            {
                result.Errors.Add("Debe introducir todos los datos.");
                result.IsValid = false;
            }

            return result;
        }
    }

    public enum Level
    {
        Rookie = 1,
        Intermediate = 2,
        Advanced =3
    }
}
