using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader product);
}
