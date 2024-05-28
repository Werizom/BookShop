using BookShop.DataAccess.Repository.IRepository;
using BookShop.Extensions;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.Constans;

namespace BookShop.Services;

public class HomeService : IHomeService
{
    private readonly IProductRepository prodRepo;
    private readonly ICategoryRepository catRepo;
    private readonly ICartService cartService;

    public HomeService(IProductRepository prodRepo, ICategoryRepository catRepo, ICartService cartService)
    {
        this.prodRepo = prodRepo;
        this.catRepo = catRepo;
        this.cartService = cartService;
    }

    public HomeViewModel GetHomeViewModel()
    {
        var homeVm = new HomeViewModel()
        {
            Products = prodRepo.GetAll(includeProperties: "Category,ApplicationType"),
            Categories = catRepo.GetAll()
        };

        return homeVm;
    }

    public DetailsViewModel GetDetailViewModel(int id, HttpContext httpContext)
    {
        var shoppingCartList = cartService.GetCartFromSession(httpContext);

        DetailsViewModel detailsViewModel = new DetailsViewModel()
        {
            Product = prodRepo.FirstOrDefault(filter: i => i.Id == id, includeProperties: "Category,ApplicationType"),
            ExistsInCart = false
        };

        foreach (var item in shoppingCartList)
        {
            if (item.ProductId == id)
            {
                detailsViewModel.ExistsInCart = true;
            }
        }

        return detailsViewModel;
    }

    public void AddItemToCart(int id, DetailsViewModel detailsView, HttpContext httpContext)
    {
        var shoppingCartList = cartService.GetCartFromSession(httpContext);

        shoppingCartList.Add(new ShoppingCart { ProductId = id, Count = detailsView.Product.TempCount });

        httpContext.Session.Set(WebConstans.SessionCart, shoppingCartList);
    }

    public void RemoveFromCart(int id, HttpContext httpContext)
    {
        var shoppingCartList = cartService.GetCartFromSession(httpContext);

        var itemToRemove = shoppingCartList.SingleOrDefault(i => i.ProductId == id);
        if (itemToRemove != null)
        {
            shoppingCartList.Remove(itemToRemove);
        }

        httpContext.Session.Set(WebConstans.SessionCart, shoppingCartList);
    }
}