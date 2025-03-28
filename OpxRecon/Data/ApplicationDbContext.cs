using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpxRecon.Models;

namespace OpxRecon.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public DbSet<Station> Stations { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<EmailServerSettings> EmailServerSettings { get; set; }
    }
}
