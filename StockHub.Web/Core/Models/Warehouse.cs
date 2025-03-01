using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.Models
{
	[Index(nameof(Name), IsUnique = true)]
	public class Warehouse : BaseModel
	{
        public Warehouse()
        {
			Items = [];
			Employees = [];
			Orders = [];
		}
		public int WarehouseId { get; set; }
		[MaxLength(100)]
		public string Name { get; set; } = null!;
		public string Location { get; set; } 
        public ICollection<Item> Items { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Order> Orders { get; set; }

        public string? ManagerId { get; set; }  // Foreign Key to IdentityUser
        public ApplicationUser? Manager { get; set; }  // Navigation property 
    }
}