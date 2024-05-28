using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Models.ViewModels;
using BookShop.Services.Interfaces;
using BookShop.Utility.Constans;

namespace BookShop.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository prodRepo;
    private readonly IWebHostEnvironment webHostEnvironment;

    public ProductService(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
    {
        this.prodRepo = prodRepo;
        this.webHostEnvironment = webHostEnvironment;
    }

    public void CreateOrUpdateProduct(ProductViewModel productViewModel, HttpContext httpContext)
    {
        var files = httpContext.Request.Form.Files;
        var webRootPath = webHostEnvironment.WebRootPath;

        if (productViewModel.Product.Id == 0)
        {
            CreateProduct(productViewModel, files, webRootPath);
        }
        else
        {
            UpdateProduct(productViewModel, files, webRootPath);
        }
    }

    private void CreateProduct(ProductViewModel productViewModel, IFormFileCollection files, string webRootPath)
    {
        string upload = webRootPath + WebConstans.ImagePath;
        string fileName = SaveFileAndGetFileName(files, upload);
        productViewModel.Product.Image = fileName;

        prodRepo.Add(productViewModel.Product);
        prodRepo.Save();
    }
    private void UpdateProduct(ProductViewModel productViewModel, IFormFileCollection files, string webRootPath)
    {
        var itemFromDb = prodRepo.FirstOrDefault(i => i.Id == productViewModel.Product.Id, isTracking: false);

        if (files.Count > 0)
        {
            string fileName = SaveFileAndGetFileName(files, webRootPath);
            productViewModel.Product.Image = fileName;

            DeleteProductImage(productViewModel.Product);
        }
        else
        {
            productViewModel.Product.Image = itemFromDb.Image;
        }

        prodRepo.Update(productViewModel.Product);
        prodRepo.Save();
    }
    private string SaveFileAndGetFileName(IFormFileCollection files, string webRootPath)
    {
        string upload = webRootPath + WebConstans.ImagePath;
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(files[0].FileName);
        using (var fileStream = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
        {
            files[0].CopyTo(fileStream);
        }
        return fileName;
    }
    private void DeleteProductImage(Product product)
    {
        string upload = Path.Combine(webHostEnvironment.WebRootPath, WebConstans.ImagePath);
        string oldFile = Path.Combine(upload, product.Image);

        if (File.Exists(oldFile))
        {
            File.Delete(oldFile);
        }
    }

    public void PrepareViewModel(ProductViewModel productVM)
    {
        productVM.CategorySelectList = prodRepo.GetAllDropdownList(WebConstans.CategoryName);
        productVM.ApplicationTypeSelectList = prodRepo.GetAllDropdownList(WebConstans.ApplicationTypeName);
    }

    public ProductViewModel InitializeProductViewModel(int? id)
    {
        var productViewModel = new ProductViewModel()
        {
            Product = (id == null) ? new Product() : prodRepo.Find(id.GetValueOrDefault()),
            CategorySelectList = prodRepo.GetAllDropdownList(WebConstans.CategoryName),
            ApplicationTypeSelectList = prodRepo.GetAllDropdownList(WebConstans.ApplicationTypeName)
        };

        return productViewModel;
    }

    public void DeleteProduct(Product item)
    {
        DeleteProductImage(item);

        prodRepo.Remove(item);
        prodRepo.Save();
    }
}