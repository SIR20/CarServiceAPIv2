using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CarServiceAPIv2.JWT
{
    public class AuthOptions
    {
        public const string ISSUER = "CarService"; // издатель токена
        public const string AUDIENCE = "CarServiceClient"; // потребитель токена
        const string KEY = "GOIha4ssdi3wJo-iD1d1oqi21";   // ключ для шифрации
        public const int LIFETIME = 4320; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
