using CarServiceAPIv2.Models;
using Microsoft.AspNetCore.Mvc;


namespace CarServiceAPIv2.Controllers
{
    public class BaseController : ControllerBase
    {
        public AppContext db;
        public BaseController(AppContext context) => db = context;
    }
}
