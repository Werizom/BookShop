using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models.Models;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Layouts;

namespace BookShop.Controllers;

[Authorize(Roles = WebConstans.AdminRole)]
public class ApplicationTypeController : Controller
{
    private readonly IApplicationTypeRepository appRepo;

    public ApplicationTypeController(IApplicationTypeRepository appRepo)
    {
        this.appRepo = appRepo;
    }

    public IActionResult Index()
    {
        IEnumerable<ApplicationType> itemList = appRepo.GetAll();
        return View(itemList);
    }

    //GET - CREATE
    public IActionResult Create()
    {
        return View();
    }

    //POST - CREATE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ApplicationType item)
    {
        if (ModelState.IsValid)
        {
            CreateApplicationType(item);
        }

        return View(item);
    }
    private IActionResult CreateApplicationType(ApplicationType item)
    {
        appRepo.Add(item);
        appRepo.Save();
        TempData[WebConstans.Success] = "Action completed successfully";

        return RedirectToAction("Index");
    }


    //GET - EDIT
    public IActionResult Edit(int? id)
    {
        if (!IsIdValid(id))
        {
            return NotFound();
        }

        var item = appRepo.Find(id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    //POST - EDIT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(ApplicationType item)
    {
        if (ModelState.IsValid)
        {
            UpdateApplicationType(item);
        }
        return View(item);

    }
    private IActionResult UpdateApplicationType(ApplicationType item)
    {
        appRepo.Update(item);
        appRepo.Save();
        TempData[WebConstans.Success] = "Action completed successfully";

        return RedirectToAction("Index");
    }


    //GET - DELETE
    public IActionResult Delete(int? id)
    {
        if (!IsIdValid(id))
        {
            return NotFound();
        }

        var item = appRepo.Find(id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        return View(item);
    }

    //POST - DELETE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var item = appRepo.Find(id.GetValueOrDefault());
        if (item == null)
        {
            return NotFound();
        }

        appRepo.Remove(item);
        appRepo.Save();
        TempData[WebConstans.Success] = "Action completed successfully";

        return RedirectToAction("Index");
    }

    private bool IsIdValid(int? id)
    {
        return id != null && id != 0;
    }
}