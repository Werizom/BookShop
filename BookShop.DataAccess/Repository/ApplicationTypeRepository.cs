using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository;

public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
{
    private readonly ApplicationsDbContext dbContext;

    public ApplicationTypeRepository(ApplicationsDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(ApplicationType applicationType)
    {
        var itemFromDb = dbContext.Categories.FirstOrDefault(i => i.Id == applicationType.Id);
        if (itemFromDb != null)
        {
            itemFromDb.Name = applicationType.Name;
        }
    }
}
