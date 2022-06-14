using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarServiceAPIv2.Models
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Administrator> Adminstators { get; set; }
        public DbSet<TaskList> TaskLists { get; set; }

        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {

            //Database.EnsureDeleted();
            Database.EnsureCreated();
            if (!Adminstators.ToList().Any(i => i.Login == "SIR")){
                Administrator sir = new Administrator()
                {
                    Login = "SIR",
                    Password = "AdminPassword"
                };

                Administrator takutokashi = new Administrator()
                {
                    Login = "Takutokashi",
                    Password = "AdminPassword"
                };

                Adminstators.AddRange(sir, takutokashi);

                SaveChanges();
            }
        }
    }
}
