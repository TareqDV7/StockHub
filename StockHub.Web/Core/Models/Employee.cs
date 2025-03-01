using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.Models
{
	public class Employee : BaseModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		[EmailAddress]
		public string Email { get; set; }
		public string Role { get; set; }
		public string Phone { get; set; }
		[Display(Name ="Warehouse")]
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
	}
}
