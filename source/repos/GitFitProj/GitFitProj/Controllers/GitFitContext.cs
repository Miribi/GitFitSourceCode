using GitFitProj.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GitFitProj.Controllers
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public class GitFitContext : DbContext
    {
        /// <summary>
        /// Changing DOB to only show date instead of DateTime.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .Property(u => u.DateOfBirth)
                .HasColumnType("date"); // This configures EF to create a column of type 'date' in the database

            // ... other model configurations ...
        }


        /// <summary>
        /// DB Configuration.
        /// </summary>
        public GitFitContext(DbContextOptions<GitFitContext> options)
        : base(options)
        {
        }


        /// <summary>
        /// get or set Usermodel.
        /// </summary>
        public DbSet<UserModel> UserModel { get; set; }
        /// <summary>
        /// get or set ActivityModel.
        /// </summary>
        public DbSet<ActivityModel> Activity { get; set; }

    }

    
}
