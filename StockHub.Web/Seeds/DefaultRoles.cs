using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.WarehouseManager));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.beneficiary));
            }
        }
    }
}
