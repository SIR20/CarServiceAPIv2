using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CarServiceAPIv2.Models.SupportModels;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/Admin")]
    public class AdminController : BaseController
    {
        IConfiguration _configuration;
        public AdminController(Models.AppContext context, IConfiguration configuration) : base(context) => _configuration = configuration;

        private string GenerateJwtToken(Administrator admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("ID", admin.Id.ToString()), new Claim("Role", "Admin") }),
                Expires = DateTime.UtcNow.AddHours(72),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("Login")]
        public IActionResult Login(string login, string password)
        {
            Administrator admin = db.Adminstators.FirstOrDefault(i => i.Login == login && i.Password == password);
            if (admin == null)
            {
                return BadRequest("Неверный логин или пароль");
            }

            string tokenString = GenerateJwtToken(admin);
            return Ok(new { Token = tokenString, UserId = admin.Id });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Manager/Managers")]
        public ActionResult<List<SupportWorker>> GetManagers()
        {
            string role = User.FindFirstValue("Role");

            if (role != "Admin")
            {
                return BadRequest(new { Status = 401 });
            }

            return Ok(db.Workers.ToList().Select(i => new SupportWorker(i.Id, i.Name, i.Login)));
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Manager/Add")]
        public IActionResult AddManager(string name, string login, string password)
        {
            string role = User.FindFirstValue("Role");

            if (role != "Admin")
            {
                return BadRequest(new { Status = 401 });
            }

            Worker worker = db.Workers.FirstOrDefault(i => i.Login == login);
            if (worker == null)
            {
                worker = new Worker()
                {
                    Name = name,
                    Login = login,
                    Password = password
                };

                db.Workers.Add(worker);
                db.SaveChanges();

                return Ok(worker.Id);
            }

            return BadRequest();
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("Manager/Delete")]
        public IActionResult DeleteManager(int managerId)
        {
            string role = User.FindFirstValue("Role");

            if (role != "Admin")
            {
                return BadRequest(new { Status = 401 });
            }

            Worker worker = db.Workers.FirstOrDefault(i => i.Id == managerId);
            if (worker != null)
            {

                db.Workers.Remove(worker);
                db.SaveChangesAsync();

                return Ok(worker.Id);
            }

            return BadRequest();
        }

    }
}
