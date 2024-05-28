using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace BookShop.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService homeService;

    public HomeController(IHomeService homeService)
    {
        this.homeService = homeService;
    }

    public IActionResult Index()
    {
        var homeViewModel = homeService.GetHomeViewModel();

        return View(homeViewModel);
    }

    public IActionResult Details(int id)
    {
        var detailsViewModel = homeService.GetDetailViewModel(id, HttpContext);

        return View(detailsViewModel);
    }

    [HttpPost, ActionName("Details")]
    public IActionResult DetailsPost(int id, DetailsViewModel detailsView)
    {
        homeService.AddItemToCart(id, detailsView, HttpContext);
        
        TempData[WebConstans.Success] = "Item add to cart successfully";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult RemoveFromCart(int id)
    {
        homeService.RemoveFromCart(id, HttpContext);

        TempData[WebConstans.Success] = "Item removed from cart successfully";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}