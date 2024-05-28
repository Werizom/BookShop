using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository;

public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
{
    private readonly ApplicationsDbContext dbContext;

    public InquiryHeaderRepository(ApplicationsDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(InquiryHeader item)
    {
        dbContext.InquiryHeaders.Update(item);
    }
}
