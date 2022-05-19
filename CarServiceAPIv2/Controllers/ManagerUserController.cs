using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;
using CarServiceAPIv2.Models.SupportModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;

namespace CarServiceAPIv2.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/ManagerUser")]
    [ApiController]
    public class ManagerUserController : BaseController
    {
        private readonly IConfiguration _configuration;
        public ManagerUserController(Models.AppContext context, IConfiguration configuration) : base(context) => _configuration = configuration;

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Users")]
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
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<User>> DeleteUser(int userId)
        {

            User user = db.Users.Where(i => i.Id == userId).FirstOrDefault();
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
        [HttpPost("UnDeleteUser")]
        public async Task<ActionResult> UnDeleteUser(int userId)
        {
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
    }
}
