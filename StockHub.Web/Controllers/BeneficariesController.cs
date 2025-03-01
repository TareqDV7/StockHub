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
    //   [Authorize(Roles =AppRoles.Admin)]
    public class BeneficariesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BeneficariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var Beneficaries = _context.Beneficiaries.Include(b => b.Warehouse).AsNoTracking().ToList();
            return View(Beneficaries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BeneficiaryFormModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var beneficiary = new Beneficiary()
            {
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                IdentityNumber = model.IdentityNumber,
                FamilyMembers = model.FamilyMembers,
                WarehouseId = model.WarehouseId,
                Warehouse = _context.Warehouses.SingleOrDefault(w => w.WarehouseId == model.WarehouseId)
            };

            _context.Beneficiaries.Add(beneficiary);
            var result = _context.SaveChanges();

            return PartialView("_BeneficaryRow", beneficiary);

        }

        public IActionResult Details(int id)
        {
            var beneficiary = _context.Beneficiaries
                .Include(w => w.Warehouse)
                .Include(w => w.Orders)
                .SingleOrDefault(w => w.Id == id);

            if (beneficiary is null)
                return NotFound();

            return View(beneficiary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id)
        {
            var beneficiary = _context.Beneficiaries.Find(id);
            if (beneficiary is null) { return NotFound(); }
            beneficiary.IsDeleted = !beneficiary.IsDeleted;
            beneficiary.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(beneficiary.LastUpdatedOn.ToString());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var beneficiary = _context.Beneficiaries.Find(id);
            if (beneficiary == null) { return NotFound(); }
            var beneficiaryVM = new BeneficiaryFormModel()
            {
                Id = id,
                Name = beneficiary.Name,
                Phone = beneficiary.Phone,
                Address = beneficiary.Address,
                IdentityNumber = beneficiary.IdentityNumber,
                FamilyMembers = beneficiary.FamilyMembers,
                ApplicationDate = beneficiary.ApplicationDate,
                WarehouseId = beneficiary.WarehouseId,
            };
            return PartialView("Form", beneficiaryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BeneficiaryFormModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var beneficiary = _context.Beneficiaries.Find(model.Id);
            if (beneficiary is null) { return NotFound(); }

            beneficiary.Name = model.Name;
            beneficiary.Phone = model.Phone;
            beneficiary.Address = model.Address;
            beneficiary.IdentityNumber = model.IdentityNumber;
            beneficiary.FamilyMembers = model.FamilyMembers;
            beneficiary.ApplicationDate = model.ApplicationDate;
            beneficiary.WarehouseId = model.WarehouseId;
            beneficiary.Warehouse = _context.Warehouses.SingleOrDefault(w => w.WarehouseId == model.WarehouseId);

            _context.SaveChanges();

            if (User.IsInRole(AppRoles.beneficiary))
            {
                return RedirectToAction(nameof(Details), new { id = beneficiary.Id });
            }
            return PartialView("_BeneficaryRow", beneficiary);
        }

        [HttpGet]
        public IActionResult RequestAnOrder(int id)
        {
            var beneficiary = _context.Beneficiaries.Include(b => b.Warehouse).SingleOrDefault(b => b.Id == id);
            if (beneficiary is null) return NotFound();

            var order = new Order
            {
                BeneficiaryId = id,
                WarehouseId = beneficiary.WarehouseId,
                RequestDate = DateTime.Now,
                FulfilledDate = DateTime.Now.AddDays(14),
                Status = "Pending",
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Order has been successfully created!";

            return RedirectToAction(nameof(Details), new { id = id });
        }

    }
}
