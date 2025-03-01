using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockHub.Web.Core.Models;
using StockHub.Web.Core.ViewModels;
using StockHub.Web.Data;
using StockHub.Web.Filters;

namespace StockHub.Web.Controllers
{
    //[Authorize(Roles = AppRoles.Admin)]

    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(Order? order)
        {

            // Separate orders by status
            var pendingOrders = _context.Orders
                .Include(o => o.Beneficiary)
                .Include(o => o.Warehouse)
                .Where(o => o.Status == "Pending")
                .ToList();
            
            if (order != null && order.Status == "Pending")
            {
                pendingOrders.Add(order);
            }

            var completedOrders = _context.Orders
                .Include(o => o.Beneficiary)
                .Include(o => o.Warehouse)
                .Where(o => o.Status == "Completed")
                .ToList();

            if (order != null && order.Status == "Completed")
            {
                completedOrders.Add(order);
            }

            // Pass data to the view using Tuple (not ValueTuple)
            var model = new Tuple<IEnumerable<Order>, IEnumerable<Order>>(pendingOrders, completedOrders);

            return View(model);
        }


        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("OrderForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var order = new Order()
            {
                RequestDate = DateTime.UtcNow,
                FulfilledDate = DateTime.UtcNow.AddDays(14),
                Status = model.Status,
                BeneficiaryId = model.BeneficiaryId,
                WarehouseId = model.WarehouseId,
            };

            order.Status = "Pending";

            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), order);
        }

        //public IActionResult AllowItem(OrderFormViewModel vm)
        //{
        //    var employee = _context.Employees.SingleOrDefault(c => c.Name == vm.Name);
        //    var isAllowed = employee is null || employee.Id.Equals(vm.Id);
        //    return Json(isAllowed);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id)
        {
            var order = _context.Orders.Find(id);
            if (order is null) { return NotFound(); }
            if(order.Status =="Pending")
            order.Status = "Completed";
            else if (order.Status == "Completed")
                order.Status = "Pending";
            order.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Json(new { LastUpdatedOn = order.LastUpdatedOn.ToString(), Status = order.Status });
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) { return NotFound(); }
            var orderVM = new OrderFormViewModel()
            {
                Id = id,
                RequestDate = order.RequestDate,
                FulfilledDate = order.FulfilledDate,
                Status = order.Status,
                BeneficiaryId = order.BeneficiaryId,
                WarehouseId = order.WarehouseId,
            };
            return PartialView("OrderForm", orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var order = _context.Orders.Find(model.Id);
            if (order is null) { return NotFound(); }

            order.RequestDate = model.RequestDate;
            order.FulfilledDate = model.FulfilledDate;
            order.BeneficiaryId = model.BeneficiaryId;
            order.WarehouseId = model.WarehouseId;
            order.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();
            if (User.IsInRole(AppRoles.beneficiary))
            {
                return RedirectToAction("Details", "Beneficaries", new{ id = order.BeneficiaryId });
            }
            return RedirectToAction(nameof(Index), order);
        }
    }
}