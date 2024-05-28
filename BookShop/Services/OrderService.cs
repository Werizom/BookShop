using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.BrainTree;
using BookShop.Utility.Constans;
using Braintree;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Services;

public class OrderService : IOrderService
{
    private readonly IOrderHeaderRepository orderHeaderRepo;
    private readonly IOrderDetailRepository orderDetailRepo;
    private readonly IBrainTreeGate brainGate;

    public OrderService(IOrderHeaderRepository orderHeaderRepo, IOrderDetailRepository orderDetailRepo, IBrainTreeGate brainGate)
    {
        this.orderHeaderRepo = orderHeaderRepo;
        this.orderDetailRepo = orderDetailRepo;
        this.brainGate = brainGate;
    }

    public OrderViewModel CreateOrderViewModel(int id)
    {
        return new OrderViewModel()
        {
            OrderHeader = orderHeaderRepo.FirstOrDefault(i => i.Id == id),
            OrderDetail = orderDetailRepo.GetAll(i => i.OrderHeaderId == id, includeProperties: "Product"),
        };
    }

    public OrderListViewModel CreateOrderListViewModel()
    {
        return new OrderListViewModel()
        {
            OrderHeaderList = orderHeaderRepo.GetAll(),
            StatusList = WebConstans.listStatus.ToList().Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            })
        };
    }

    public void FilteredOrders(OrderListViewModel orderListViewModel, string searchName, 
        string searchEmail, string searchPhone, string Status)
    {
        if (!string.IsNullOrEmpty(searchName))
        {
            FilterOrdersByName(orderListViewModel, searchName);
        }
        if (!string.IsNullOrEmpty(searchEmail))
        {
            FilterOrdersByEmail(orderListViewModel, searchEmail);
        }
        if (!string.IsNullOrEmpty(searchPhone))
        {
            FilterOrdersByPhone(orderListViewModel, searchPhone);
        }
        if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
        {
            FilterOrdersByStatus(orderListViewModel, Status);
        }
    }

    private void FilterOrdersByName(OrderListViewModel orderListViewModel, string searchName)
    {
        orderListViewModel.OrderHeaderList = orderListViewModel.OrderHeaderList
            .Where(i => i.FullName.ToLower().Contains(searchName.ToLower()));
    }
    private void FilterOrdersByEmail(OrderListViewModel orderListViewModel, string searchEmail)
    {
        orderListViewModel.OrderHeaderList = orderListViewModel.OrderHeaderList
            .Where(i => i.Email.ToLower().Contains(searchEmail.ToLower()));
    }
    private void FilterOrdersByPhone(OrderListViewModel orderListViewModel, string searchPhone)
    {
        orderListViewModel.OrderHeaderList = orderListViewModel.OrderHeaderList
            .Where(i => i.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
    }
    private void FilterOrdersByStatus(OrderListViewModel orderListViewModel, string status)
    {
        orderListViewModel.OrderHeaderList = orderListViewModel.OrderHeaderList
            .Where(i => i.OrderStatus.ToLower().Contains(status.ToLower()));
    }

    public void UpdateOrderStatus(string status, OrderViewModel orderViewModel)
    {
        var orderId = orderViewModel.OrderHeader.Id;
        var orderHeader = orderHeaderRepo.FirstOrDefault(i => i.Id == orderId);
        if (orderHeader != null)
        {
            orderHeader.OrderStatus = status;
            if (status == WebConstans.StatusShipped)
            {
                orderHeader.ShippingDate = DateTime.Now;
            }
            orderHeaderRepo.Save();
        }
    }

    public void CancelOrder(OrderViewModel orderViewModel)
    {
        var orderHeader = orderHeaderRepo.FirstOrDefault(i => i.Id == orderViewModel.OrderHeader.Id);

        OrderCanceled(orderHeader);

        orderHeader.OrderStatus = WebConstans.StatusRefunded;
        orderHeaderRepo.Save();
    }

    private void OrderCanceled(OrderHeader orderHeader)
    {
        var gateway = brainGate.GetGateway();
        var transaction = gateway.Transaction.Find(orderHeader.TransactionId);

        if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
        {
            // no refund
            var resultVoid = gateway.Transaction.Void(orderHeader.TransactionId);
        }
        else
        {
            //refund
            var resultRefaund = gateway.Transaction.Refund(orderHeader.TransactionId);
        }
    }

    public OrderHeader UpdateOrderDetails(OrderViewModel orderViewModel)
    {
        var orderHeaderFromDb = orderHeaderRepo.FirstOrDefault(i => i.Id == orderViewModel.OrderHeader.Id);
        UpdateOrderHeaderDetails(orderHeaderFromDb, orderViewModel);
        orderHeaderRepo.Save();

        return orderHeaderFromDb;
    }

    private void UpdateOrderHeaderDetails(OrderHeader orderHeaderFromDb, OrderViewModel orderViewModel)
    {
        orderHeaderFromDb.FullName = orderViewModel.OrderHeader.FullName;
        orderHeaderFromDb.Email = orderViewModel.OrderHeader.Email;
        orderHeaderFromDb.PhoneNumber = orderViewModel.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = orderViewModel.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = orderViewModel.OrderHeader.City;
        orderHeaderFromDb.State = orderViewModel.OrderHeader.State;
        orderHeaderFromDb.PostalCode = orderViewModel.OrderHeader.PostalCode;
    }
}