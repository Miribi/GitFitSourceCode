using GitFitProj.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GitFitProj.Controllers
{
    public class DataBaseController : DbContext
    {
        public DataBaseController(DbContextOptions<DataBaseController> options)
        : base(options)
        {
        }

        public DbSet<UserModel> UserModel { get; set; }
        public DbSet<ActivityModel> Activity { get; set; }

    }

    
}
