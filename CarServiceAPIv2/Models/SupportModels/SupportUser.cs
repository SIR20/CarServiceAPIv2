using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models.SupportModels
{
    public class SupportUser : BaseModel
    {
        public SupportUser(int id, string name, string number, bool isBan)
        {
            Id = id;
            Name = name;
            Number = number;
            IsBan = isBan;
        }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsBan { get; set; }
    }
}
