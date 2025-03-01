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

    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarehousesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var warehouses = _context.Warehouses.AsNoTracking().ToList();
            return View(warehouses);
        }


        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WarehouseFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var warehouse = new Warehouse()
            {
                Name = model.Name,
                Location = model.Location,
            };

            _context.Warehouses.Add(warehouse);
            var result = _context.SaveChanges();

            return PartialView("_WarehouseRow", warehouse);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse == null) { return NotFound(); }
            var warehouseVM = new WarehouseFormViewModel()
            {
                Id = id,
                Name = warehouse.Name,
                Location = warehouse.Location,
            };
            return PartialView("_Form", warehouseVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromBody] WarehouseFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var warehouse = _context.Warehouses.Find(model.Id);
            if (warehouse is null) { return NotFound(); }

            warehouse.Name = model.Name;
            warehouse.Location = model.Location;
            warehouse.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();
            if (User.IsInRole(AppRoles.WarehouseManager))
            {
                return RedirectToAction(nameof(Details), new { id = warehouse.WarehouseId });
            }
            return PartialView("_WarehouseRow", warehouse);
        }


        public async Task<IActionResult> Details(int id)
        {
            var warehouse = await _context.Warehouses
                .Include(w => w.Manager)
                .Include(w => w.Items)
                .Include(w => w.Employees)
                .Include(w => w.Orders)
                .FirstOrDefaultAsync(w => w.WarehouseId == id);

            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id)
        {
            var warehouse = _context.Warehouses.Find(id);
            if (warehouse is null) { return NotFound(); }
            warehouse.IsDeleted = !warehouse.IsDeleted;
            warehouse.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(warehouse.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(WarehouseFormViewModel vm)
        {
            var warehouse = _context.Warehouses.SingleOrDefault(c => c.Name == vm.Name);
            var isAllowed = warehouse is null || warehouse.WarehouseId.Equals(vm.Id);
            return Json(isAllowed);
        }

        //public IActionResult AssignManager(int warehouseId, string managerName)
        //{
        //    var warehouse = _context.Warehouses
        //        .Include(w => w.Manager)
        //        .SingleOrDefault(w => w.WarehouseId == warehouseId);

        //    if (warehouse is null)
        //        return NotFound();

        //    // Create and assign manager
        //    var manager = new WarehouseManager
        //    {
        //        Name = managerName,
        //        WarehouseId = warehouseId,
        //        Warehouse = warehouse
        //    };

        //    warehouse.Manager = manager;

        //    _context.Warehouses.Update(warehouse);
        //    _context.SaveChanges();

        //    // Redirect to Details
        //    return RedirectToAction(nameof(Details), new { id = warehouseId });
        //}



    }
}