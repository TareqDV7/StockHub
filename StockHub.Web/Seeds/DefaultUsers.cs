using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockHub.Web.Core.Models;
using StockHub.Web.Data;

namespace Bookify.Web.Seeds
{
    public class DefaultUsers
    {
        public static async Task SeedAdminUsersAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = new()
            {
                UserName = "admin",
                Email = "admin@StockHub.com",
                FullName = "Admin",
                EmailConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(admin.Email);

            if (user is null)
            {
                await userManager.CreateAsync(admin, "P@ssword123");
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }

        }


        public static async Task SeedBeneficiaryUsersAsync(IServiceScopeFactory scopeFactory, ILogger logger)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Add a Beneficiary user
            string beneficiaryUserName = "beneficiary1";
            string beneficiaryEmail = "beneficiary1@example.com";
            string beneficiaryFullName = "Beneficiary One";
            string beneficiaryPassword = "P@ssword123";
            string beneficiaryName = "Beneficiary Name";
            string beneficiaryAddress = "Beneficiary Address";
            string beneficiaryIdentityNumber = "123456789";

            // Create a test warehouse, or retrieve one from the database
            var warehouse = context.Warehouses.FirstOrDefault();
            if (warehouse == null)
            {
                warehouse = new Warehouse()
                {
                    Name = "Test Warehouse",
                    Location = "Test Location"
                };
                context.Warehouses.Add(warehouse);
                await context.SaveChangesAsync();
            }
            // Get or create a user
            ApplicationUser beneficiaryUser = new()
            {
                UserName = beneficiaryUserName,
                Email = beneficiaryEmail,
                FullName = beneficiaryFullName,
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(beneficiaryUser.Email);

            if (user == null)
            {
                var result = await userManager.CreateAsync(beneficiaryUser, beneficiaryPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(beneficiaryUser, AppRoles.beneficiary);
                }
                else
                {
                    // Handle errors during user creation (e.g., log them)
                    foreach (var error in result.Errors)
                    {
                        logger.LogError(error.Description);
                    }
                }
            }
            else
            {
                beneficiaryUser = user;
            }

            // Check to see if the user already has a beneficiary relationship.
            var hasExistingBeneficiary = await context.Beneficiaries.AnyAsync(b => b.UserId == beneficiaryUser.Id);
            if (!hasExistingBeneficiary)
            {
                // First, create the Beneficiary record, linked to the correct user.
                Beneficiary beneficiary = new()
                {
                    Name = beneficiaryName,
                    Address = beneficiaryAddress,
                    IdentityNumber = beneficiaryIdentityNumber,
                    Phone = beneficiaryIdentityNumber,
                    FamilyMembers = 2,
                    WarehouseId = warehouse.WarehouseId,
                    UserId = beneficiaryUser.Id
                };
                context.Beneficiaries.Add(beneficiary);
                await context.SaveChangesAsync();
                // Set the beneficiary ID, which is required on the ApplicationUser model.
                beneficiaryUser.BeneficiaryId = beneficiary.Id;
                await userManager.UpdateAsync(beneficiaryUser);
            }
            else
            {
                logger.LogInformation($"Beneficiary not created for user {beneficiaryUser.Email}, because they already have a relationship.");
            }

        }

        public static async Task SeedWarehouseManagersAsync(IServiceScopeFactory scopeFactory, ILogger logger)
        {
            using var scope = scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext name


            // Add a Warehouse Manager user
            string managerUserName = "manager1";
            string managerEmail = "manager1@example.com";
            string managerFullName = "Manager One";
            string managerPassword = "P@ssword123";

            // Create a test warehouse, or retrieve one from the database
            var warehouse = context.Warehouses.FirstOrDefault();
            if (warehouse == null)
            {
                warehouse = new Warehouse()
                {
                    Name = "Test Warehouse 1",
                    Location = "Test Location 1"
                };
                context.Warehouses.Add(warehouse);
                await context.SaveChangesAsync();
            }
            var warehouse2 = context.Warehouses.FirstOrDefault(w => w.Name == "Test Warehouse 2");
            if (warehouse2 == null)
            {
                warehouse2 = new Warehouse()
                {
                    Name = "Test Warehouse 2",
                    Location = "Test Location 2"
                };
                context.Warehouses.Add(warehouse2);
                await context.SaveChangesAsync();
            }


            // Get or create a user
            ApplicationUser managerUser = new()
            {
                UserName = managerUserName,
                Email = managerEmail,
                FullName = managerFullName,
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(managerUser.Email);

            if (user == null)
            {
                var result = await userManager.CreateAsync(managerUser, managerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, AppRoles.WarehouseManager);
                }
                else
                {
                    // Handle errors during user creation (e.g., log them)
                    foreach (var error in result.Errors)
                    {
                        logger.LogError(error.Description);
                    }
                    return;
                }
            }
            else
            {
                managerUser = user;
            }

            if (warehouse.ManagerId == null)
            {
                // Assign the user to the warehouse
                warehouse.ManagerId = managerUser.Id;
                managerUser.WarehouseId = warehouse.WarehouseId;
                await context.SaveChangesAsync();
                await userManager.UpdateAsync(managerUser);
                logger.LogInformation($"Warehouse manager assigned to {warehouse.Name} successfully");
            }
            else
            {
                logger.LogInformation($"Warehouse manager for {warehouse.Name} already exists");
            }
            if (warehouse2.ManagerId == null)
            {
                // Assign the user to the warehouse
                warehouse2.ManagerId = managerUser.Id;
                managerUser.WarehouseId = warehouse2.WarehouseId;
                await context.SaveChangesAsync();
                await userManager.UpdateAsync(managerUser);
                logger.LogInformation($"Warehouse manager assigned to {warehouse2.Name} successfully");
            }
            else
            {
                logger.LogInformation($"Warehouse manager for {warehouse2.Name} already exists");
            }
        }
    }
}