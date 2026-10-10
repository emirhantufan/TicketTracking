using Microsoft.EntityFrameworkCore;
using TicketTracking.Models;

namespace TicketTracking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tİckets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
    }
}



