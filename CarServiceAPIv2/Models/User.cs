﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models
{
    public class User : BaseModel
    {
        public string Name { get; set; } = "None";
        public string Number { get; set; }
        public string Password { get; set; }
        public bool IsBan { get; set; }
    }
}
