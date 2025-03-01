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

    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeesController(ApplicationDbContext context)
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
            return PartialView("EmployeeForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var employee = new Employee()
            {
                Name = model.Name,
                Email = model.Email,
                Role = model.Role,
                Phone = model.Phone,
                WarehouseId = model.WarehouseId,
            };

            _context.Employees.Add(employee);
             _context.SaveChanges();
            return PartialView("_EmployeeRow", employee);
        }

        public IActionResult AllowItem(EmployeeFormViewModel vm)
        {
            var employee = _context.Employees.SingleOrDefault(c => c.Name == vm.Name);
            var isAllowed = employee is null || employee.Id.Equals(vm.Id);
            return Json(isAllowed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee is null) { return NotFound(); }
            employee.IsDeleted = !employee.IsDeleted;
            employee.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(employee.LastUpdatedOn.ToString());
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null) { return NotFound(); }
            var employeeVM = new EmployeeFormViewModel()
            {
                Id = id,
                Name = employee.Name,
                Email = employee.Email,
                WarehouseId = employee.WarehouseId,
                Phone = employee.Phone,
                Role = employee.Role
            };
            return PartialView("EmployeeForm", employeeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest();
            }

            var employee = _context.Employees.Find(model.Id);
            if (employee is null) { return NotFound(); }

            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.Phone = model.Phone;
            employee.Role = model.Role;
            employee.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return PartialView("_EmployeeRow", employee);
        }
    }
}
