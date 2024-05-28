using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository;

public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
{
    private readonly ApplicationsDbContext dbContext;

    public InquiryDetailRepository(ApplicationsDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(InquiryDetail item)
    {
        dbContext.InquiryDetails.Update(item);
    }
}
