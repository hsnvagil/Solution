using App.Core.Interfaces;
using App.Infrastructure.Logger;
using App.Infrastructure.Providers;
using App.Infrastructure.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure;

public static class ConfigureServices {
    public static void AddInfrastructureServices(this IServiceCollection services) {
        services.AddSingleton<IRequestResponseLogger, FileRequestResponseLogger>();
        services.AddSingleton<IClock, AzerbaijanClock>();
        services.AddScoped<IHomeDataProvider, InMemoryHomeDataProvider>();
        services.AddScoped<IRequestResponseLogCreator, RequestResponseLogCreator>();
    }
}