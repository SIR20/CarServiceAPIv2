using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/Task")]
    [ApiController]
    public class ClientTaskController : BaseController
    {
        public ClientTaskController(AppContext context) : base(context) { }

        [HttpPost("Create")]
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

        [HttpGet("Info")]
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
    }
}
