using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.ViewModels
{
    public class ItemFormViewModel
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }

        [MaxLength(100, ErrorMessage = "Name's length can't be more than 100 charachters ")]
        [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = "There is an item with the same name")]
        public string Name { get; set; } = null!;
        public string Contents { get; set; } = null!;
        public int Quantity { get; set; } 
    }
}
