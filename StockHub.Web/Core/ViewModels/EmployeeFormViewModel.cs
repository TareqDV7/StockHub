using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }
        [Display(Name ="Warehouse")]
        public int WarehouseId { get; set; }

        [MaxLength(100, ErrorMessage = "Name's length can't be more than 100 charachters ")]
        [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = "There is an employee with the same name")]
        public string Name { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Phone { get; set; }
    }
}
