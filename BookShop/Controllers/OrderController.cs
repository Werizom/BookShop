using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.BrainTree;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers;

[Authorize(Roles = WebConstans.AdminRole)]
public class OrderController : Controller
{
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
        this.orderService = orderService;
    }

    [BindProperty]
    public OrderViewModel OrderViewModel { get; set; }


    [HttpGet]
    public IActionResult Index(
        string searchName = null,
        string searchEmail = null,
        string searchPhone = null,
        string Status = null)
    {
        var orderListViewModel = orderService.CreateOrderListViewModel();
        orderService.FilteredOrders(orderListViewModel, searchName, searchEmail, searchPhone, Status);

        return View(orderListViewModel);
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        OrderViewModel = orderService.CreateOrderViewModel(id);

        return View(OrderViewModel);
    }

    [HttpPost]
    public IActionResult StartProcessing()
    {
        orderService.UpdateOrderStatus(WebConstans.StatusInProcess, OrderViewModel);
        SetSuccessMessageForTempData("Order is In Process");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ShipOrder()
    {
        orderService.UpdateOrderStatus(WebConstans.StatusShipped, OrderViewModel);
        SetSuccessMessageForTempData("Order Shipped Successfully");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult CancelOrder()
    {
        orderService.CancelOrder(OrderViewModel);
        SetSuccessMessageForTempData("Order Cancelled Successfully");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult UpdateOrderDetails()
    {
        var orderHeaderFromDb = orderService.UpdateOrderDetails(OrderViewModel);
        SetSuccessMessageForTempData("Order Details Updated Successfully");

        return RedirectToAction("Details", "Order", new { id = orderHeaderFromDb.Id });
    }

    private void SetSuccessMessageForTempData(string message)
    {
        TempData[WebConstans.Success] = message;
    }
}