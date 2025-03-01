using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.ViewModels
{
    public class OrderFormViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        public DateTime RequestDate { get; set; } 
        public DateTime? FulfilledDate { get; set; } 
        public string? Status { get; set; } 
        public int BeneficiaryId { get; set; } 
    }
}
