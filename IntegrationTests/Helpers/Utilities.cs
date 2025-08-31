using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helpers
{
    public static class Utilities
    {
        public static async Task ResetLinesAsync(ApplicationDbContext db, bool emptyTable = false)
        {
            db.Lines.RemoveRange(db.Lines);

            if (!emptyTable)
            {
                db.Lines.AddRange(GetSeedingLines());
            }
            await db.SaveChangesAsync();
        }

        public static async Task ResetUsersAsync(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            bool emptyTable = false)
        {
            var existingUsers = db.Users.Include(u => u.Subscriptions).ToList();
            foreach (var u in existingUsers)
                u.Subscriptions.Clear();

            db.Users.RemoveRange(existingUsers);
            await db.SaveChangesAsync();

            var users = GetSeedingUsers();
            if (!emptyTable)
            {
                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "Password123!");
                    if (!result.Succeeded)
                        throw new Exception($"Failed to seed user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        public static List<Line> GetSeedingLines()
        {
            return new List<Line>
            {
                new() { Id = "bakerloo", Code = "BL", Name = "Bakerloo Line", Color = "#894E24" },
                new() { Id = "central", Code = "CL", Name = "Central Line", Color = "#E32017" },
            };
        }

        public static List<ApplicationUser> GetSeedingUsers()
        {
            return new List<ApplicationUser>
            {
                new() { UserName = "existinguser1", Email = "existing1@example.com", FullName = "Existing1 User", Id = Guid.Parse("11111111-1111-1111-1111-111111111111")},
                new() { UserName = "existinguser2", Email = "existing2@example.com", FullName = "Existing2 User ", Id = Guid.Parse("21111111-1111-1111-1111-111111111111")}
            };
        }
    }
}
