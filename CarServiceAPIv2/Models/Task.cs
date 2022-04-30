using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models
{
    public class Task : BaseModel
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
    }
}
