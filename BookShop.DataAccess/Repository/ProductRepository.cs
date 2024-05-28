using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationsDbContext dbContext;

    public ProductRepository(ApplicationsDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<SelectListItem> GetAllDropdownList(string item)
    {
        if(item == WebConstans.CategoryName)
        {
            return dbContext.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        if (item == WebConstans.ApplicationTypeName)
        {
            return dbContext.ApplicationTypes.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        return null;
    }

    public void Update(Product product)
    {
        dbContext.Update(product);
    }
}
