using BookShop.Models.Models;
using BookShop.Models.ViewModels;

namespace BookShop.Services.Interfaces;

public interface IProductService
{
    void CreateOrUpdateProduct(ProductViewModel productViewModel, HttpContext httpContext);
    void PrepareViewModel(ProductViewModel productVM);
    ProductViewModel InitializeProductViewModel(int? id);
    void DeleteProduct(Product item);
}