using FileGuard.Storage.Files.Providers;
using FileGuard.Storage.Files.Services;
using FileGuard.Storage.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileGuard.Storage.Files.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var fileStorageSettings = new FileStorageSettings();
        configuration.GetSection(nameof(FileStorageSettings)).Bind(fileStorageSettings);
        services.AddSingleton(fileStorageSettings);

        services.AddScoped<IFileWriter, FileStorageService>();
        services.AddScoped<IFileReader, FileStorageService>();

        services.AddScoped<IKeysProvider, FileStorageKeysProvider>();
        services.AddScoped<IFolderService, FolderService>();

        return services;
    }
}
