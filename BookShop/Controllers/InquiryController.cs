using BookShop.DataAccess.Repository.IRepository;
using BookShop.Extensions;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers;

[Authorize(Roles = WebConstans.AdminRole)]
public class InquiryController : Controller
{
    private readonly IInquiryHeaderRepository inquiryHeaderRepo;
    private readonly IInquiryDetailRepository inquiryDetailRepo;

    [BindProperty]
    public InquiryViewModel InquiryViewModel { get; set; }

    public InquiryController(IInquiryHeaderRepository inquiryHeaderRepo, IInquiryDetailRepository inquiryDetailRepo)
    {
        this.inquiryHeaderRepo = inquiryHeaderRepo;
        this.inquiryDetailRepo = inquiryDetailRepo;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        InquiryViewModel = GetInquiryViewModel(id);
       
        return View(InquiryViewModel);
    }

    private InquiryViewModel GetInquiryViewModel(int id)
    {
        return new InquiryViewModel
        {
            InquiryHeader = inquiryHeaderRepo.FirstOrDefault(i => i.Id == id),
            InquiryDetail = inquiryDetailRepo.GetAll(i => i.InquiryHeaderId == id, includeProperties: "Product")
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Details()
    {
        InquiryViewModel.InquiryDetail = inquiryDetailRepo.GetAll(i => i.InquiryHeaderId == InquiryViewModel.InquiryHeader.Id);
        
        var shoppingCartList = CreateShoppingCartList(InquiryViewModel);

        SetSession(shoppingCartList);

        return RedirectToAction("Index", "Cart");
    }

    private List<ShoppingCart> CreateShoppingCartList(InquiryViewModel inquiryViewModel)
    {
        var shoppingCartList = new List<ShoppingCart>();

        foreach (var detail in inquiryViewModel.InquiryDetail)
        {
            var shoppingCart = new ShoppingCart
            {
                ProductId = detail.ProductId
            };

            shoppingCartList.Add(shoppingCart);
        }

        return shoppingCartList;
    }

    private void SetSession(List<ShoppingCart> shoppingCartList)
    {
        HttpContext.Session.Clear();
        HttpContext.Session.Set(WebConstans.SessionCart, shoppingCartList);
        HttpContext.Session.Set(WebConstans.SessionInquiryId, InquiryViewModel.InquiryHeader.Id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete()
    {
        DeleteInquiry();
        TempData[WebConstans.Success] = "Action completed successfully";

        return RedirectToAction(nameof(Index));
    }

    private void DeleteInquiry()
    {
        var inquiryHeader = inquiryHeaderRepo.FirstOrDefault(i => i.Id == InquiryViewModel.InquiryHeader.Id);
        IEnumerable<InquiryDetail> inquiryDetails = inquiryDetailRepo.GetAll(i => i.InquiryHeaderId == InquiryViewModel.InquiryHeader.Id);

        inquiryDetailRepo.RemoveRange(inquiryDetails);
        inquiryHeaderRepo.Remove(inquiryHeader);
        inquiryHeaderRepo.Save();

    }

    // APICALLS

    [HttpGet]
    public IActionResult GetInquiryList()
    {
        return Json(new { data = inquiryHeaderRepo.GetAll() });
    }

}