using Microsoft.EntityFrameworkCore;


namespace DotNet5WebApp.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var v = this;

            string s = Startup.Configuration.GetSection("ConnectionStrings")["Database"];

            optionsBuilder.UseSqlServer(Startup.Configuration.GetSection("ConnectionStrings")["Database"]);
        }

        public DbSet<Project> Projects { get; set; }
    }
}
