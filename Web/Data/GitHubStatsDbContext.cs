using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
{
    public class GitHubStatsDbContext : DbContext
    {
        public GitHubStatsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<GitHubRepository> Repositories { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=data.db");
    }
}