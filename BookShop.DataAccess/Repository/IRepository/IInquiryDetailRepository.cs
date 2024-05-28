using BookShop.Models.Models;

namespace BookShop.DataAccess.Repository.IRepository;

public interface IInquiryDetailRepository : IRepository<InquiryDetail>
{
    void Update(InquiryDetail product);
}
