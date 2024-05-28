using BookShop.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Models.ViewModels;

public class OrderListViewModel
{
    public IEnumerable<OrderHeader> OrderHeaderList { get; set; }
    public IEnumerable<SelectListItem> StatusList { get; set; }
    public string Status { get; set; }
}
