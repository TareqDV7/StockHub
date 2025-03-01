using StockHub.Web.Core.Models;

namespace StockHub.Web.Core.ViewModels
{
    public class WarehouseEmployeesViewModel
    {
        public string WarehouseName { get; set; } = string.Empty;
        public IEnumerable<Employee> Employees { get; set; } = Enumerable.Empty<Employee>();
    }
}
