using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Astor.Proxy.ExampleApi
{
    public class Jwt
    {
        public const string Key = "mylongandsupersecretkeyforexampleproject";
        public static byte[] KeyBytes => Encoding.ASCII.GetBytes(Key);
        public const string Audience = "anyone";
        public const string Issuer = "me";

        public static string Create()
        {
            var securityKey = new SymmetricSecurityKey(KeyBytes);
            return new JwtSecurityTokenHandler().CreateEncodedJwt(new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                Audience = Audience,
                Issuer = Issuer,
                SigningCredentials = new SigningCredentials(securityKey, "HS256")
            });
        }
    }
}