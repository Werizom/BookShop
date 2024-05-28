using BookShop.Models.Models;

namespace BookShop.Models.ViewModels;

public class OrderViewModel
{
    public OrderHeader OrderHeader { get; set; }
    public IEnumerable<OrderDetail> OrderDetail { get; set; }
}
