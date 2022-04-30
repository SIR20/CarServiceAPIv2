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

        [HttpPost("CreateCar")]
        public async Task<ActionResult> CreateCar(string model, string number, int userId)
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
                return BadRequest("Внутреняя ошибка сервера");
            }
        }

        [HttpGet("CarInfo")]
        public ActionResult<Car> CarInfo(int carId)
        {
            try
            {
                return db.Cars.Where(i => i.Id == carId).ToList().Last();
            }
            catch
            {
                return BadRequest("Внутрення ошибка сервера");
            }
        }

        [HttpGet("UserCars")]
        public ActionResult<List<Car>> UserCars(int userId)
        {
            try
            {
                return db.Cars.Where(i => i.UserId == userId).ToList();
            }
            catch
            {
                return BadRequest("Внутрення ошибка сервера");
            }
        }
    }
}
