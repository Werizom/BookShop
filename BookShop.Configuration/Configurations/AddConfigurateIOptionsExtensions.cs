using BookShop.Utility.BrainTree;
using BookShop.Utility.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BookShop.Configuration.Configurations;

public static class AddConfigurateIOptionsExtensions
{
    public static IServiceCollection AddConfigurateIOptions(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<BrainTreeOptions>(configuration);
        //services.Configure<DbOptions>(configuration);

        services.Configure<BrainTreeOptions>(configuration.GetSection("BrainTree"));
        services.Configure<MailJetOptions>(configuration.GetSection("MailJet"));

        return services;
    }
}
