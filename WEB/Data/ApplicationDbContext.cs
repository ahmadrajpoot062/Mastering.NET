using Mastering.NET.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Mastering.NET.Data
{
    public class ApplicationDbContext : IdentityDbContext<myAppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Core.Entities.Lecture> Lecture { get; set; } = default!;
    }
}
