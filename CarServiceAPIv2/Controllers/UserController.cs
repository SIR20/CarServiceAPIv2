using CarServiceAPIv2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(AppContext context) : base(context) { }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(string number, string password)
        {
            if (db.Users.Any(i => i.Number == number)) return BadRequest("Пользователь с таким номером уже существует");

            User user = new User()
            {
                Number = number,
                Password = password
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return Ok(user.Id);
        }

        [HttpPost("LoginUser")]
        public ActionResult LoginUser(string number,string password)
        {
            try
            {
                User user = db.Users.Where(i => i.Number == number && i.Password == password).ToList().Last();
                return Ok(user.Id);
            }
            catch
            {
                return BadRequest("Неверный логин или пароль");
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string number,string oldPassword,string newPassword)
        {
            try
            {
                User user = db.Users.Where(i => i.Number == number && i.Password == oldPassword).ToList().Last();
                user.Password = newPassword;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Неверный старый пароль");
            }
        }
    }
}
