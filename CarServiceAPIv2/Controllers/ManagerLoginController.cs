using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/Manager")]
    [ApiController]
    public class ManagerLoginController : BaseController
    {
        private readonly IConfiguration _configuration;
        public ManagerLoginController(Models.AppContext context, IConfiguration configuration) : base(context) => _configuration = configuration;

        private string GenerateJwtToken(Worker worker)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("ID", worker.Id.ToString()), new Claim("Role", "Manager") }),
                Expires = DateTime.UtcNow.AddHours(72),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("Login")]
        public ActionResult Login(string login, string password)
        {
            Worker worker = db.Workers.Where(i => i.Login == login && i.Password == password).FirstOrDefault();
            if (worker != null)
            {
                string tokenString = GenerateJwtToken(worker);
                return Ok(new { Token = tokenString, WorkerId = worker.Id });
            }

            return BadRequest("Неверный логин или пароль");
        }
    }
}
