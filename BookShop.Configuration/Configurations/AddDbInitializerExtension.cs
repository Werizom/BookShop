using BookShop.DataAccess.Initializer;
using Microsoft.Extensions.DependencyInjection;

namespace BookShop.Configuration.Configurations;

public static class  AddDbInitializerExtension
{
    public static void AddDbInitializer(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var dbInitializer = serviceProvider.GetRequiredService<IDbInitializer>();

        dbInitializer.Initialize();
    }
}