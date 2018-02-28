using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupClubLib.Model
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            Errors = new List<string>();
        }
        public List<string> Errors { get; private set; }
        public bool IsValid { get; set; }
    }
}
