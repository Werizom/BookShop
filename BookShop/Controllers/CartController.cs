using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.BrainTree;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly IOrderHeaderRepository orderHeaderRepo;
    private readonly IBrainTreeGate brainGate;
    private readonly ICartService cartService;

    [BindProperty]
    public ProductUserViewModel ProductUserViewModel { get; set; }

    public CartController(IOrderHeaderRepository orderHeaderRepo, IBrainTreeGate brainGate, ICartService cartService)
    {
        this.orderHeaderRepo = orderHeaderRepo;
        this.brainGate = brainGate;
        this.cartService = cartService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var shoppingCartList = cartService.GetCartFromSession(HttpContext);
        var productList = cartService.GenerateProductList(shoppingCartList);

        return View(productList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Index")]
    public IActionResult IndexPost(IEnumerable<Product> products)
    {
        cartService.UpdateCountInCart(products, HttpContext);

        return RedirectToAction(nameof(Summary));
    }

    [HttpGet]
    public IActionResult Summary()
    {
        ApplicationUser applicationUser;
        
        if (User.IsInRole(WebConstans.AdminRole))
        {
            applicationUser = cartService.GetUserForAdmin(HttpContext);
            GenerateBraintreeClientToken(brainGate);
        }
        else
        {
            applicationUser = cartService.GetUserForRegularUser(User);
        }

        var shoppingCart = cartService.GetCartFromSession(HttpContext);
        cartService.GetListProductFromDBBasedOnCart(shoppingCart);

        ProductUserViewModel = cartService.GetListFromCartUserViewModel(applicationUser, shoppingCart, ProductUserViewModel);

        return View(ProductUserViewModel);
    }

    private void GenerateBraintreeClientToken(IBrainTreeGate brainGate)
    {
        var clientToken = brainGate.GetClientToken(brainGate);

        ViewBag.ClientToken = clientToken;
    }

    [HttpPost]
    [ActionName("Summary")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserViewModel productUserViewModel)
    {
        var claim = cartService.GetClaimIdentityUser(User);

        if (User.IsInRole(WebConstans.AdminRole))
        {
            return await ProcessAdminOrder(collection, productUserViewModel, claim);
        }
        else
        {
            return await ProcessUserInquiry(productUserViewModel, claim);
        }
    }

    private async Task<IActionResult> ProcessAdminOrder(IFormCollection collection, ProductUserViewModel productUserViewModel, Claim claim)
    {
        var resultOrderHeader = cartService.ProcessAdminOrder(collection, productUserViewModel, claim);

        return RedirectToAction(nameof(InquiryConfirmation), new { id = resultOrderHeader.Id });
    }
    private async Task<IActionResult> ProcessUserInquiry(ProductUserViewModel productUserViewModel, Claim claim)
    {
        cartService.ProcessUserInquiry(productUserViewModel, claim);

        TempData[WebConstans.Success] = "Inquiry submitted successfully";

        return RedirectToAction(nameof(InquiryConfirmation));
    }

    public IActionResult InquiryConfirmation(int id = 0)
    {
        var orderHeader = orderHeaderRepo.FirstOrDefault(i => i.Id == id);
        
        HttpContext.Session.Clear();

        return View();
    }

    public IActionResult Remove(int id)
    {
        cartService.Remove(id, HttpContext);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Clear()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateCart(IEnumerable<Product> products)
    {
        cartService.UpdateCountInCart(products, HttpContext);

        return RedirectToAction(nameof(Index));
    }

}