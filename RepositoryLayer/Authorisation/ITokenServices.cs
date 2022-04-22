using CommonLayer.Model;
using JWT;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Authorisation
{
    public interface ITokenServices
    {

        string GetToken(string userId, string email);

        TokenValidation ValidateToken(HttpRequest req);
    }
}
