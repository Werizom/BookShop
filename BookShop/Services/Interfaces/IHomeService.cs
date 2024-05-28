using BookShop.Models.ViewModels;

namespace BookShop.Services.Interfaces;

public interface IHomeService
{
    HomeViewModel GetHomeViewModel();
    DetailsViewModel GetDetailViewModel(int id, HttpContext httpContext);
    void AddItemToCart(int id, DetailsViewModel detailsView, HttpContext httpContext);
    void RemoveFromCart(int id, HttpContext httpContext);
}