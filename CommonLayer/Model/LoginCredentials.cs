using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;

namespace CommonLayer.Model
{
    public class LoginCredentials
    {
        public UserDetails userDetails { get; set; }

        public string token { get; set; }
    }
}
