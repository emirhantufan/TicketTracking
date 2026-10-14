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
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.CommentedByUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(tc => tc.CommentedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}