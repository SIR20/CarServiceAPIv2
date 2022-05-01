using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;

namespace CarServiceAPIv2.Controllers
{
    [Route("api/Car")]
    [ApiController]
    public class CarController : BaseController
    {
        public CarController(AppContext context) : base(context) { }

        [HttpPost("Create")]
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

        [HttpGet("Info")]
        public ActionResult<Car> Info(int carId)
        {
            try
            {
                return db.Cars.Where(i => i.Id == carId).FirstOrDefault();
            }
            catch
            {
                return BadRequest("Внутренняя ошибка сервера");
            }
        }

        [HttpGet("UserCars")]
        public ActionResult<Dictionary<int,string>> UserCars(int userId)
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
