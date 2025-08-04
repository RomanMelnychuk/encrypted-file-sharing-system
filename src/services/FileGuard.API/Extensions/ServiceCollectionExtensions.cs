using FileGuard.API.Providers;
using FileGuard.Crypto.Extensions;
using FileGuard.Identity.Application.Extensions;
using FileGuard.Identity.DataAccess.Extensions;
using FileGuard.Storage.Files.Extensions;

namespace FileGuard.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureFileStorage(configuration);
        services.ConfigureCryptoServices(configuration);
        services.ConfigureIdentity(configuration);
        services.ConfigureApplicationLayer(configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<IUserProvider, UserProvider>();

        return services;
    }
}
