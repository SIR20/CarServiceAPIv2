using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;
using CarServiceAPIv2.Models.SupportModels;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/ManagerUser")]
    [ApiController]
    public class ManagerUserController : BaseController
    {
        public ManagerUserController(AppContext context) : base(context) { }

        [HttpGet("Users")]
        public ActionResult<List<SupportUser>> GetUsers()
        {
            return db.Users
                .Select(i => new SupportUser(i.Id, i.Name, i.Number, i.IsBan))
                .ToList();
        }

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
