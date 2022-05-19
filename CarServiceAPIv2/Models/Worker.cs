using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models
{
    public class Worker : BaseModel
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
