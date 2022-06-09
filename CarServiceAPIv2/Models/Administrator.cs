using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models
{
    public class Administrator: BaseModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
