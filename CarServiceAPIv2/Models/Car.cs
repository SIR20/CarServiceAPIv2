using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models
{
    public class Car : BaseModel
    {
        public string Model { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public int UserId { get; set; }
    }
}
