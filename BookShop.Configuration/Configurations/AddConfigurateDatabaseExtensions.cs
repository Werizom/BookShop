using BookShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookShop.Configuration.Configurations;

public static class AddConfigurateDatabaseExtensions
{
    public static IServiceCollection AddConfigurationDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationsDbContext>(options => 
            options.UseSqlServer(
                configuration.GetConnectionString("BookShopContext") 
                    ?? throw new InvalidOperationException("Connection string 'BookShopContext' not found.")));

        return services;
    }
}