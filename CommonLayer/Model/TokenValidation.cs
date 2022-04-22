using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class TokenValidation
    {
        public bool IsValid { get; set; }

        public string UserId { get; set; } = "";

        public string Email { get; set; } = "";

    }
}
