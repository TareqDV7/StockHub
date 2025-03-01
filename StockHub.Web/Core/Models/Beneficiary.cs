using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.Models
{
	public class Beneficiary : BaseModel
	{
        public Beneficiary()
        {
			Orders = [];
        }

        public int Id { get; set; }
		public string Name { get; set; } = null!;
        public string Phone { get; set; }
        public string Address { get; set; }
		public string? Status { get; set; } // active , inactive
        [Required]
        [RegularExpression("^[1-9]{9}$", ErrorMessage = "Identity Number must be 9 digits.")]
        public string IdentityNumber { get; set; }
        [Required]
        [Range(1, 20, ErrorMessage = "Family members must be between 1 and 20.")]
        public int FamilyMembers { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
		public DateTime? ApplicationDate { get; set; }
		public DateTime? CollectionDate { get; set; }
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }





    }
}