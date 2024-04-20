using GitFitProj.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GitFitProj.Controllers
{
    public class GitFitContext : DbContext
    {
        public GitFitContext(DbContextOptions<GitFitContext> options)
        : base(options)
        {
        }

        public DbSet<UserModel> UserModel { get; set; }
        public DbSet<ActivityModel> Activity { get; set; }

    }

    
}
