using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using System.Security.Claims;

namespace BookShop.Services.Interfaces;

public interface ICartService
{
    List<ShoppingCart> GetCartFromSession(HttpContext httpContext);
    ApplicationUser GetUserForRegularUser(ClaimsPrincipal user);
    Claim GetClaimIdentityUser(ClaimsPrincipal user);
    List<Product> GenerateProductList(List<ShoppingCart> shoppingCartList);
    List<ShoppingCart> UpdateCountInCart(IEnumerable<Product> products, HttpContext httpContext);
    ApplicationUser GetUserForAdmin(HttpContext httpContext);
    void GetListProductFromDBBasedOnCart(List<ShoppingCart> shoppingCart);
    ProductUserViewModel GetListFromCartUserViewModel(ApplicationUser applicationUser, List<ShoppingCart> shoppingCart, ProductUserViewModel productUserViewModel);
    Task<OrderHeader> ProcessAdminOrder(IFormCollection collection, ProductUserViewModel productUserViewModel, Claim claim);
    Task ProcessUserInquiry(ProductUserViewModel productUserViewModel, Claim claim);
    void Remove(int id, HttpContext httpContext);


}