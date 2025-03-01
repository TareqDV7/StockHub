namespace StockHub.Web.Core.Models
{
    public class Order : BaseModel
    {
        public int OrderId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? FulfilledDate { get; set; }//        public DateTime? FulfilledDate => RequestDate.AddDays(14);
        public string Status { get; set; } // e.g., Pending, Completed
        public int BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

    }
}