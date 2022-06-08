using Microsoft.EntityFrameworkCore;

namespace CarServiceAPIv2.Models
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Administrator> Adminstators { get; set; }

        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {

            //Database.EnsureDeleted();
            Database.EnsureCreated();
            Administrator sir = new Administrator()
            {
                Name = "SIR",
                Number = "9534906725",
                Password = "AdminPassword"
            };

            Administrator takutokashi = new Administrator()
            {
                Name = "Takutokashi",
                Number = "9314977339",
                Password = "AdminPassword"
            };

            Adminstators.AddRange(sir, takutokashi);
            SaveChanges();
        }
    }
}
