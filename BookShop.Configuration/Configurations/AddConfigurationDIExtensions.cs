using BookShop.DataAccess.Initializer;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.DataAccess.Repository;
using BookShop.Utility.BrainTree;
using BookShop.Utility.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookShop.Configuration.Configurations;

public static class AddConfigurationDIExtensions
{
    public static IServiceCollection AddConfigurationDI(this IServiceCollection services)
    {

        services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
        services.AddTransient<IEmailSender, EmailSender>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
        services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}