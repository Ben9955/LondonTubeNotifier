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

            var lines = new List<Line>
            {
                new Line { Id = "bakerloo", Code = "BL", Name = "Bakerloo", Color = "#B36305" },
                new Line { Id = "central", Code = "CL", Name = "Central", Color = "#E32017" },
                new Line { Id = "circle", Code = "CC", Name = "Circle", Color = "#FFD300" },
                new Line { Id = "district", Code = "DL", Name = "District", Color = "#00782A" },
                new Line { Id = "hammersmith-city", Code = "HCL", Name = "Hammersmith & City", Color = "#F3A9BB" },
                new Line { Id = "jubilee", Code = "JL", Name = "Jubilee", Color = "#6A7278" },
                new Line { Id = "metropolitan", Code = "ML", Name = "Metropolitan", Color = "#9B0056" },
                new Line { Id = "northern", Code = "NL", Name = "Northern", Color = "#000000" },
                new Line { Id = "piccadilly", Code = "PL", Name = "Piccadilly", Color = "#0019A8" },
                new Line { Id = "victoria", Code = "VL", Name = "Victoria", Color = "#0098D4" },
                new Line { Id = "waterloo-city", Code = "WCL", Name = "Waterloo & City", Color = "#95CDBA" },
           };

            modelBuilder.Entity<Line>().HasData(lines);

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
