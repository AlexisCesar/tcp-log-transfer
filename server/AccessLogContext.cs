using Microsoft.EntityFrameworkCore;

namespace server
{
    internal class AccessLogContext : DbContext
    {
        public DbSet<AccessLogRegister> accessLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: "Server=localhost,1433;Database=MyDatabase;User Id=sa;Password=Database!2022;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessLogRegister>().HasKey(p => p.Id);
        }

    }
}
