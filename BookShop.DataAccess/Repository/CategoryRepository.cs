using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationsDbContext dbContext;

    public CategoryRepository(ApplicationsDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(Category category)
    {
        var itemFromDb = dbContext.Categories.FirstOrDefault(i => i.Id == category.Id);
        if (itemFromDb != null)
        {
            itemFromDb.Name = category.Name;
            itemFromDb.DisplayOrder = category.DisplayOrder;
        }
    }
}
