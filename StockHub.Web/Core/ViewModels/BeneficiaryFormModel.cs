    namespace StockHub.Web.Core.ViewModels
{
    public class BeneficiaryFormModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IdentityNumber { get; set; }
        public int FamilyMembers { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? CollectionDate { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Id { get; set; }


    }
}
