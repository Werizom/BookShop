using BookShop.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Models.ViewModels;

public class ProductViewModel
{
    public Product Product { get; set; }
    public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    public IEnumerable<SelectListItem> ApplicationTypeSelectList { get; set; }
}