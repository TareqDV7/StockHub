using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.Models
{
	public class Item : BaseModel
	{
		public int Id { get; set; }
		[MaxLength(100)]
		public string Name { get; set; } = null!;
		[MaxLength(500)]
		public string Contents { get; set; } = null!;
		public int Quantity { get; set; }
		[Display(Name ="Warehouse")]
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
	}
}
