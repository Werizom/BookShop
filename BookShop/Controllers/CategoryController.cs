using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers;

[Authorize(Roles = WebConstans.AdminRole)]
public class CategoryController : Controller
{
    private readonly ICategoryRepository catRepo;

    public CategoryController(ICategoryRepository catRepo)
    {
        this.catRepo = catRepo;
    }

    public IActionResult Index()
    {
        IEnumerable<Category> categoriesList = catRepo.GetAll();

        return View(categoriesList);
    }

    //Get - create
    public IActionResult Create()
    {
        return View();
    }


    //Post - create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category category)
    {

        if(ModelState.IsValid)
        {
            CreateProduct(category);
        }

        TempData[WebConstans.Error] = "Error while creating category";

        return View(category);
    }

    private IActionResult CreateProduct(Category category)
    {
        catRepo.Add(category);
        catRepo.Save();
        TempData[WebConstans.Success] = "Category created sucessfully";

        return RedirectToAction("Index");
    }

    //Get edit
    public IActionResult Edit(int? id)
    {
        if (!IsIdValid(id))
        {
            return NotFound();
        }

        var item = catRepo.Find(id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    //Post - edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            catRepo.Update(category);
            catRepo.Save();

            return RedirectToAction("Index");
        }

        return View(category);
    }


    //Get delete
    public IActionResult Delete(int? id)
    {
        if (!IsIdValid(id))
        {
            return NotFound();
        }

        var item = catRepo.Find(id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    //Post - delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? Id)
    {
        var item = catRepo.Find(Id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        catRepo.Remove(item);
        catRepo.Save();

        return RedirectToAction("Index");
    }

    private bool IsIdValid(int? id)
    {
        return id != null && id != 0;
    }
}