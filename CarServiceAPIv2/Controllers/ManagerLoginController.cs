using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/Manager")]
    [ApiController]
    public class ManagerLoginController : BaseController
    {
        public ManagerLoginController(AppContext context) : base(context) { }

        [HttpPost("Login")]
        public ActionResult Login(string login, string password)
        {
            Worker worker = db.Workers.Where(i => i.Login == login && i.Password == password).FirstOrDefault();
            if (worker != null)
            {
                return Ok(worker.Id);
            }

            return BadRequest("Неверный логин или пароль");
        }
    }
}
