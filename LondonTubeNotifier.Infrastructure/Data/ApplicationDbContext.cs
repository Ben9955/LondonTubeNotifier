using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Data
{
    public class ApplicationDbContext : 
        IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions option) : base(option) { }
        public ApplicationDbContext() { }

        public DbSet<Line> Lines { get; set; }
        public DbSet<LineStatus> LineStatuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many between ApplicationUser and Line
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Subscriptions)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserLineSubscription"));

            // One-to-many between Line and LineStatus
            modelBuilder.Entity<LineStatus>()
                .HasOne(ls => ls.Line)
                .WithMany(l => l.LineStatuses)
                .HasForeignKey(ls => ls.LineId);

            modelBuilder.Entity<LineStatus>()
                .Property(ls => ls.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
