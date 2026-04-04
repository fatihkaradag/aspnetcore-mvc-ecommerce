using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_mvc_ecommerce.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task InitializeAsync()
        {
            // Apply pending migrations — abort seeding if migration fails
            try
            {
                var strategy = _db.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    var pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                        await _db.Database.MigrateAsync();
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Migration failed: {ex.Message}");
                // Do not proceed with seeding if DB schema is not ready
                return;
            }

            // Seed roles if they do not already exist
            await EnsureRoleAsync(SD.Role_Admin);
            await EnsureRoleAsync(SD.Role_Employee);
            await EnsureRoleAsync(SD.Role_Customer);
            await EnsureRoleAsync(SD.Role_Company);

            // Seed default users
            await EnsureUserAsync("admin@example.com", "Admin123!", SD.Role_Admin);
            await EnsureUserAsync("employee@example.com", "Employee123!", SD.Role_Employee);
            await EnsureUserAsync("company@example.com", "Company123!", SD.Role_Company);
            await EnsureUserAsync("customer@example.com", "Customer123!", SD.Role_Customer);
        }

        // Creates role only if it does not already exist
        private async Task EnsureRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Creates IdentityUser with email/password and assigns role; skips if already exists
        private async Task EnsureUserAsync(string email, string password, string role)
        {
            // Skip seeding if user already exists
            if (await _userManager.FindByEmailAsync(email) != null) return;

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.Error.WriteLine($"Failed to seed user {email}: {errors}");
            }
        }
    }
}