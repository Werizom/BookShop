using BookShop.DataAccess.Data;
using BookShop.Models.Models;
using BookShop.Utility.Constans;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationsDbContext db;
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public DbInitializer(ApplicationsDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.db = db;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public void Initialize()
    {
        try
        {
            if (db.Database.GetPendingMigrations().Count() > 0)
            {
                db.Database.Migrate();
            }
            else
            {
                return;
            }
        }
        catch (Exception ex)
        {

        }

        if (!roleManager.RoleExistsAsync(WebConstans.AdminRole).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(WebConstans.AdminRole)).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole(WebConstans.CustomerRole)).GetAwaiter().GetResult();
        }

        userManager.CreateAsync(new ApplicationUser
        {
            UserName = "admin@site.com",
            Email = "admin@site.com",
            EmailConfirmed = true,
            FullName = "Admin Tester",
            PhoneNumber = "111111111111"
        }, "Qwerty123!").GetAwaiter().GetResult();

        ApplicationUser user = db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@site.com");
        userManager.AddToRoleAsync(user, WebConstans.AdminRole).GetAwaiter().GetResult();

    }
}
