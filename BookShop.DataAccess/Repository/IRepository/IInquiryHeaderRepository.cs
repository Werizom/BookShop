using BookShop.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.DataAccess.Repository.IRepository;

public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
{
    void Update(InquiryHeader product);
}
