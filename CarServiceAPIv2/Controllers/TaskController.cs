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
    public class TaskController : BaseController
    {
        public TaskController(AppContext context) : base(context) { }

        [HttpPost("CreateTask")]
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

        [HttpGet("TaskInfo")]
        public ActionResult<Models.Task> TaskInfo(int taskId)
        {
            return db.Tasks.Where(i => i.Id == taskId).ToList().Last();
        }
    }
}
