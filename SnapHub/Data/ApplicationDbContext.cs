using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SnapHub.Models;

namespace SnapHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SnapHub.Models.Session>? Session { get; set; }
        public DbSet<SnapHub.Models.Portfolio>? Portfolio { get; set; }
    }
}