using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository.IRepository;

public interface IApplicationTypeRepository : IRepository<ApplicationType>
{
    void Update(ApplicationType applicationType);
}
