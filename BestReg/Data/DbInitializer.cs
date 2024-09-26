using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BestReg.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Apply any pending migrations and create the database if needed
            await context.Database.MigrateAsync();

            // Seed roles
            if (!roleManager.Roles.Any())
            {
                await SeedRolesAsync(roleManager);
            }

            // Seed admin user
            if (!userManager.Users.Any(u => u.UserName == "admin@gmail.com"))
            {
                await SeedAdminUserAsync(userManager, roleManager);
            }
        }
        public static async Task SyncExistingUsersToFirebase(UserManager<ApplicationUser> userManager)
        {
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                try
                {
                    await CreateUserInFirebase(user.Email, "DefaultPassword123!"); // Use a secure method to set passwords
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating user {user.Email}: {ex.Message}");
                }
            }
        }

        private static async Task CreateUserInFirebase(string email, string password)
        {
            var userRecordArgs = new UserRecordArgs()
            {
                Email = email,
                EmailVerified = false,
                Password = password,
                Disabled = false,
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
            Console.WriteLine($"Successfully created new user: {userRecord.Uid}");
        }  

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
        "Admin",
        "SchoolAuthority",
        "BusDriver",
        "Student"
      
    };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Error creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            Console.WriteLine("Roles seeded successfully.");
        }


        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "User",
                IDNumber = "123456789012",
                LockoutEnabled = false,  // Ensure the account is not locked out
                EmailConfirmed = true, // Skip email confirmation for admin
                QrCodeBase64 = null
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@1234");
            if (result.Succeeded)
            {
                Console.WriteLine("Admin user created successfully.");

                // Check if the Admin role exists
                if (await roleManager.RoleExistsAsync("Admin"))
                {
                    // Assign admin role
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (addToRoleResult.Succeeded)
                    {
                        Console.WriteLine("Admin user added to Admin role successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error adding admin user to Admin role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine("Admin role does not exist. Admin user was created but not assigned to any role.");
                }
            }
            else
            {
                // Log error messages using your logging framework
                Console.WriteLine($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

    }

}
