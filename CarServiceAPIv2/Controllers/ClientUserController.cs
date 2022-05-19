using CarServiceAPIv2.JWT;
using CarServiceAPIv2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class ClientUserController : BaseController
    {
        private readonly IConfiguration _configuration;
        public ClientUserController(Models.AppContext context, IConfiguration configuration) : base(context) => _configuration = configuration;

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("ID", user.Id.ToString()), new Claim("Number",user.Number) }),
                Expires = DateTime.UtcNow.AddHours(72),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create(string number, string password)
        {
            if (db.Users.Any(i => i.Number == number)) return BadRequest("Пользователь с таким номером уже существует");
            User user = new User()
            {
                Number = number,
                Password = password
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            string tokenString = GenerateJwtToken(user);
            return Ok(new { Token = tokenString, UserId = user.Id });
        }

        [HttpPost("Login")]
        public IActionResult Login(string number, string password)
        {
            User user = db.Users.FirstOrDefault(i => i.Number == number && i.Password == password);
            if (user == null)
            {
                return BadRequest("Неверный логин или пароль");
            }

            string tokenString = GenerateJwtToken(user);
            return Ok(new { Token = tokenString, UserId = user.Id });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            string number = User.FindFirstValue("Number");
            User user = db.Users.Where(i => i.Number == number && i.Password == oldPassword).FirstOrDefault();
            if (user != null)
            {
                user.Password = newPassword;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Неверный старый пароль");
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("ChangeName")]
        public async Task<ActionResult> ChangeName(string newName)
        {
            User user = db.Users.Where(i => i.Id.ToString() == User.FindFirstValue("Id")).FirstOrDefault();
            if (user != null)
            {
                user.Name = newName;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok(user.Name);
            }

            return BadRequest("Пользователь не найден");
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("ChangeNumber")]
        public async Task<ActionResult> ChangeNumber(string newNumber)
        {
            string number = User.FindFirstValue("Number");
            User user = db.Users.Where(i => i.Number == number).FirstOrDefault();
            if (user != null)
            {
                user.Number = newNumber;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Пользователь не найден");
        }
    }
}
