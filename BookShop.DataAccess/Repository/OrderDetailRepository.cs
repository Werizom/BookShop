using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationsDbContext dbContext;

    public OrderDetailRepository(ApplicationsDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(OrderDetail item)
    {
        dbContext.OrderDetails.Update(item);
    }
}
