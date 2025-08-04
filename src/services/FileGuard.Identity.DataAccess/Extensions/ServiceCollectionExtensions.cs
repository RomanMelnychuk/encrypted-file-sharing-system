using FileGuard.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileGuard.Identity.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FileGuardDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("FileGuardDbConnectionString"));
        });

        return services;
    }
}
