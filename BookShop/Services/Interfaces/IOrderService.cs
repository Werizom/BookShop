using BookShop.Models.Models;
using BookShop.Models.ViewModels;

namespace BookShop.Services.Interfaces;

public interface IOrderService
{
    OrderViewModel CreateOrderViewModel(int id);
    OrderListViewModel CreateOrderListViewModel();
    void FilteredOrders(OrderListViewModel orderListViewModel, string searchName, string searchEmail, string searchPhone, string Status);
    void UpdateOrderStatus(string status, OrderViewModel orderViewModel);
    void CancelOrder(OrderViewModel orderViewModel);
    OrderHeader UpdateOrderDetails(OrderViewModel orderViewModel);
}