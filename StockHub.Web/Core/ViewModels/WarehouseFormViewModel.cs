using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.ViewModels
{
    public class WarehouseFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(100,ErrorMessage ="Name's length can't be more than 100 charachters ")]
		[Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = "There is a warehouse with the same name")]
		public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;
    }
}
