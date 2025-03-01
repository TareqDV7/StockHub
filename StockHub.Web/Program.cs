using Bookify.Web.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockHub.Web.Core.Models;
using StockHub.Web.Data;
using Microsoft.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<ILogger, Logger<Program>>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var scopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
    // Get the Logger service using dependency injection
    var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await DefaultRoles.SeedRolesAsync(roleManager);
    await DefaultUsers.SeedAdminUsersAsync(userManager);
    // Pass the scopeFactory and the logger for dependency injection to work
    await DefaultUsers.SeedBeneficiaryUsersAsync(scopeFactory, logger);
    await DefaultUsers.SeedWarehouseManagersAsync(scopeFactory, logger);
}


//var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

//using var scope = scopeFactory.CreateScope();

//var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//await DefaultRoles.SeedRolesAsync(roleManager);
//await DefaultUsers.SeedAdminUsersAsync(userManager);
//await DefaultUsers.SeedBeneficiaryUserAsync(scopeFactory, logger);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
