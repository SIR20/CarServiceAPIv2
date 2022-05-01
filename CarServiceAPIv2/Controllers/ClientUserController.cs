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

namespace CarServiceAPIv2.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class ClientUserController : BaseController
    {
        public ClientUserController(Models.AppContext context) : base(context) { }

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
            return Ok(user.Id);
        }

        [HttpPost("Login")]
        public ActionResult Login(string number, string password)
        {
            return Ok();
        }


        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(string number, string oldPassword, string newPassword)
        {

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

        [HttpPut("ChangeName")]
        public async Task<ActionResult> ChangeName(string number, string newName)
        {
            User user = db.Users.Where(i => i.Number == number).FirstOrDefault();
            if (user != null)
            {
                user.Name = newName;
                db.Update(user);
                await db.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("Пользователь не найден");
        }

        [HttpPut("ChangeNumber")]
        public async Task<ActionResult> ChangeNumber(string number, string newNumber)
        {
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
