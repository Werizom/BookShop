using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services;
using BookShop.Services.Interfaces;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers;

[Authorize(Roles = WebConstans.AdminRole)]
public class ProductController : Controller
{
    private readonly IProductRepository prodRepo;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IProductService productService;

    public ProductController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
    {
        this.prodRepo = prodRepo;
        this.webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = prodRepo.GetAll(includeProperties: "Category,ApplicationType");

        return View(productList);
    }

    //Get - create, edit
    [HttpGet]
    public IActionResult Upsert(int? id)
    {
        var productViewModel = productService.InitializeProductViewModel(id);
        if (productViewModel == null)
        {
            return NotFound();
        }

        return View(productViewModel);
    }

    //Post - create, edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel productViewModel)
    {
        if (!ModelState.IsValid)
        {
            productService.CreateOrUpdateProduct(productViewModel, HttpContext);
            SetSuccessMessageForTempData("Action completed successfully");

            return RedirectToAction("Index");
        }

        productService.PrepareViewModel(productViewModel);

        return View(productViewModel);
    }

    //GET DELETE
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var product = prodRepo.FirstOrDefault(i => i.Id == id, includeProperties: "Category, ApplicationType");
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    //POST - DELETE
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? Id)
    {
        var item = prodRepo.Find(Id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        productService.DeleteProduct(item);
        SetSuccessMessageForTempData("Action completed successfully");

        return RedirectToAction("Index");
    }

    private void SetSuccessMessageForTempData(string message) //TODO: зробити один загальний метод в сервісі
    {
        TempData[WebConstans.Success] = message;
    }
}