using FileGuard.Crypto.Interfaces;
using FileGuard.Storage.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileGuard.Crypto.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCryptoServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IFileEncryptor, CryptoService>();
        services.AddScoped<IFileDecryptor, CryptoService>();
        services.AddScoped<ICryptoClient, CryptoClient>();

        return services;
    }
}
