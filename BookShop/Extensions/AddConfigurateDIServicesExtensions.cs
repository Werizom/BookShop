using BookShop.Services;
using BookShop.Services.Interfaces;

namespace BookShop.Extensions;

public static class AddConfigurateDIServicesExtensions
{
    public static IServiceCollection AddConfigurationDIForServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}