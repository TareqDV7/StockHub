using StockHub.Web.Core.Models;

namespace StockHub.Web.Core.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalWarehouses { get; set; }
        public int TotalItems { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBeneficiaries { get; set; }
        public int CollectedBeneficiaries { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int ArchivedOrders { get; set; }
    }
}
