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
using CarServiceAPIv2.Models.SupportModels;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/Manager")]
    [ApiController]
    public class ManagerController : BaseController
    {
        private readonly IConfiguration _configuration;
        public ManagerController(Models.AppContext context, IConfiguration configuration) : base(context) => _configuration = configuration;

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
            Worker worker = db.Workers.FirstOrDefault(i => i.Login == login && i.Password == password);
            if (worker != null)
            {
                string tokenString = GenerateJwtToken(worker);
                return Ok(new { Token = tokenString, WorkerId = worker.Id });
            }

            return BadRequest(new { Status = 1001 });
        }


        //User
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("User/Users")]
        public ActionResult<List<SupportUser>> GetUsers()
        {
            string role = User.FindFirstValue("Role");
            if (role != "Manager")
                return BadRequest(new { Status = 401 });

            return db.Users
                    .Select(i => new SupportUser(i.Id, i.Name, i.Number, i.IsBan))
                    .ToList();
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("User/Delete")]
        public async Task<ActionResult<User>> DeleteUser(int userId)
        {
            string role = User.FindFirstValue("Role");
            if (role != "Manager")
                return BadRequest(new { Status = 401 });

            User user = db.Users.FirstOrDefault(i => i.Id == userId);
            if (user != null)
            {
                if (user.IsBan) db.Users.Remove(user);
                else user.IsBan = true;
                await db.SaveChangesAsync();
                return Ok(user);
            }


            return BadRequest("Пользователь не найден");

        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("User/UnDelete")]
        public async Task<ActionResult> UnDeleteUser(int userId)
        {
            string role = User.FindFirstValue("Role");
            if (role != "Manager")
                return BadRequest(new { Status = 401 });

            User user = db.Users.Where(i => i.Id == userId).FirstOrDefault();
            if (user != null)
            {
                user.IsBan = false;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Пользователь не найден");
        }

        //Task
    }
}
