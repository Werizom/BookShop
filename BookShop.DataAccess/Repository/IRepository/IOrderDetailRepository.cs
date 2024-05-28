using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail product);
}
