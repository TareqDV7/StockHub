using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockHub.Web.Core.ViewModels;
using StockHub.Web.Data;

namespace StockHub.Web.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]

    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalWarehouses = _context.Warehouses.Count();
            var totalItems = _context.Items.Count();
            var totalEmployees = _context.Employees.Count();
            var totalOrders = _context.Orders.Count();
            var totalBeneficiaries = _context.Beneficiaries.Count();
            var collectedBeneficiaries = _context.Beneficiaries.Count(b => b.Status == "Collected");

            // Fetch the orders and group by status for chart data
            var orders = _context.Orders.ToList();
            var pendingOrders = orders.Count(o => o.Status == "Pending");
            var completedOrders = orders.Count(o => o.Status == "Completed");
            var archivedOrders = orders.Count(o => o.Status == "Archived");

            var items = _context.Items.Include(i=>i.Warehouse).ToList(); // Fetch items for the table
            var employees = _context.Employees.ToList(); // Fetch employees for the table

            // Create a view model
            var model = new DashboardViewModel
            {
                TotalWarehouses = totalWarehouses,
                TotalItems = totalItems,
                TotalEmployees = totalEmployees,
                TotalOrders = totalOrders,
                TotalBeneficiaries = totalBeneficiaries,
                CollectedBeneficiaries = collectedBeneficiaries,
                Items = items,
                Employees = employees,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                ArchivedOrders = archivedOrders
            };

            return View(model);
        }

    }

}

