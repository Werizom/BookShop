using BookShop.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Data;

public class ApplicationsDbContext : IdentityDbContext
{
    public ApplicationsDbContext(DbContextOptions<ApplicationsDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<ApplicationType> ApplicationTypes { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;
    public DbSet<InquiryHeader> InquiryHeaders { get; set; } = default!;
    public DbSet<InquiryDetail> InquiryDetails { get; set; } = default!;
    public DbSet<OrderHeader> OrderHeaders { get; set; } = default!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = default!;

}