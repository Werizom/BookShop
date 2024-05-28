using BookShop.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
    IEnumerable<SelectListItem> GetAllDropdownList(string item);
}
