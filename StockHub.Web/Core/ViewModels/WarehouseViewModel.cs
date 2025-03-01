using System.ComponentModel.DataAnnotations;

namespace StockHub.Web.Core.ViewModels
{
    public class WarehouseViewModel
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; }
    }
}
