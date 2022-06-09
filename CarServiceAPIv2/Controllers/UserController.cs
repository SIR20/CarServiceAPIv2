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
using Microsoft.AspNetCore.Cors;

namespace CarServiceAPIv2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/User")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IConfiguration _configuration;
        public UserController(Models.AppContext context, IConfiguration configuration) : base(context) => _configuration = configuration;

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("ID", user.Id.ToString()), new Claim("Number",user.Number), new Claim("Role", "User") }),
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

        //Task
        [HttpPost("Task/Create")]
        public async Task<ActionResult> CreateTask(string type, string description, int userId, int carId)
        {
            Models.Task task = new Models.Task()
            {
                Type = type,
                Description = description,
                UserId = userId,
                CarId = carId,
                Status = "Waiting"
            };
            await db.Tasks.AddAsync(task);
            await db.SaveChangesAsync();
            return Ok(task.Id);
        }

        [HttpGet("Task/Info")]
        public ActionResult<Models.Task> TaskInfo(int taskId)
        {
            try
            {
                return db.Tasks.Where(i => i.Id == taskId).FirstOrDefault();
            }
            catch
            {
                return BadRequest("Задачи не найдено");
            }
        }


        //Car
        [HttpPost("Car/Create")]
        public async Task<ActionResult> Create(string model, string number, int userId)
        {
            try
            {
                Car car = new Car()
                {
                    Model = model,
                    Name = "None",
                    Number = number,
                    UserId = userId
                };

                await db.Cars.AddAsync(car);
                await db.SaveChangesAsync();
                return Ok(car.Id);
            }
            catch
            {
                return BadRequest("Внутренняя ошибка сервера");
            }
        }

        [HttpGet("Car/Info")]
        public ActionResult<Car> Info(int carId)
        {
            try
            {
                return db.Cars.FirstOrDefault(i => i.Id == carId);
            }
            catch
            {
                return BadRequest("Внутренняя ошибка сервера");
            }
        }

        [HttpGet("Car/UserCars")]
        public ActionResult<Dictionary<int, string>> UserCars(int userId)
        {
            try
            {
                return db.Cars.Where(i => i.UserId == userId).Select(i => new { i.Id, i.Model }).ToDictionary(i => i.Id, i => i.Model);
            }
            catch
            {
                return BadRequest("Пользователь не найден");
            }
        }
    }
}
