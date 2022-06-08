using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAPIv2.Models.SupportModels
{
    public class SupportWorker : BaseModel
    {
        public SupportWorker(int id, string name, string login)
        {
            Id = id;
            Name = name;
            Login = login;
        }
        public string Name { get; set; }
        public string Login { get; set; }
    }
}
