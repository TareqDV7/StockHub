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
   // [Authorize(Roles = AppRoles.Admin)]

    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("ItemForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ItemFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var item = new Item()
            {
                Name = model.Name,
                Contents = model.Contents,
                Quantity = model.Quantity,
                WarehouseId = model.WarehouseId,
            };

            _context.Items.Add(item);
            _context.SaveChanges();
            return PartialView("_ItemRow", item);
        }

        public IActionResult AllowItem(ItemFormViewModel vm)
        {
            var item = _context.Items.SingleOrDefault(c => c.Name == vm.Name);
            var isAllowed = item is null || item.Id.Equals(vm.Id);
            return Json(isAllowed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id)
        {
            var item = _context.Items.Find(id);
            if (item is null) { return NotFound(); }
            item.IsDeleted = !item.IsDeleted;
            item.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(item.LastUpdatedOn.ToString());
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null) { return NotFound(); }
            var itemVM = new ItemFormViewModel()
            {
                Id = id,
                Name = item.Name,
                Quantity = item.Quantity,
                Contents = item.Contents,
                WarehouseId = item.WarehouseId,
            };
            return PartialView("ItemForm", itemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ItemFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var item = _context.Items.Find(model.Id);
            if (item is null) { return NotFound(); }

            item.Name = model.Name;
            item.Contents = model.Contents;
            item.Quantity = model.Quantity;
            item.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return PartialView("_ItemRow", item);
        }
    }
}
