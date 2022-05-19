using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarServiceAPIv2.Models;

namespace CarServiceAPIv2.Controllers
{
    [Route("Admin")]
    public class AdminController : BaseController
    {
        public AdminController(Models.AppContext context) : base(context) { }
    }
}
