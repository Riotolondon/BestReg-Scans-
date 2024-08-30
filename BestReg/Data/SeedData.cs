using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task EnsureRoles(RoleManager<IdentityRole> roleManager, string[] roles)
    {
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
