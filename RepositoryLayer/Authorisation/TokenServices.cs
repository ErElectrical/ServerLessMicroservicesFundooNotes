using CommonLayer.Model;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Authorisation
{
    public class TokenServices : ITokenServices
    {
        private readonly IJwtAlgorithm _algorithm;
        private readonly IJsonSerializer _serializer;
        private readonly IBase64UrlEncoder _base64Encoder;
        private readonly IJwtEncoder _jwtEncoder;
        public TokenServices()
        {
            // JWT specific initialization.
            _algorithm = new HMACSHA256Algorithm();
            _serializer = new JsonNetSerializer();
            _base64Encoder = new JwtBase64UrlEncoder();
            _jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
        }
        public string GetToken(string userId, string email)
        {

            Dictionary<string, object> claims = new Dictionary<string, object>
            {
                
                     { "UserId", userId},
                     { "Email", email }
                
            };
            string token = _jwtEncoder.Encode(claims, "ldbudnsvjdnvbsvhsiksncbcvsgwhkaamcbvbdguhrjfkfnmdnbsvwhkmdbbdhjdm dbbdjdkmdnbbjjkdmsnsbdbh");
            return token;

        }

        public TokenValidation ValidateToken(HttpRequest req)
        {
            TokenValidation validationResponse = new TokenValidation();

            if (!req.Headers.ContainsKey("token"))
            {
                validationResponse.IsValid = false;

                return validationResponse;
            }

            string authorizationHeader = req.Headers["token"];

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                validationResponse.IsValid = false;

                return validationResponse;
            }

            IDictionary<string, object> claims = null;

            try
            {
                if (authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }

                // Validate the token and decode the claims.
                claims = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret("ldbudnsvjdnvbsvhsiksncbcvsgwhkaamcbvbdguhrjfkfnmdnbsvwhkmdbbdhjdm dbbdjdkmdnbbjjkdmsnsbdbh")
                    .MustVerifySignature()
                    .Decode<IDictionary<string, object>>(authorizationHeader);
            }
            catch (Exception)
            {
                validationResponse.IsValid = false;

                return validationResponse;
            }

            if (!claims.ContainsKey("UserId"))
            {
                validationResponse.IsValid = false;

                return validationResponse;
            }

            validationResponse.IsValid = true;
            validationResponse.UserId = Convert.ToString(claims["UserId"]);
            validationResponse.Email = Convert.ToString(claims["Email"]);

            return validationResponse;




        }
    }
}
