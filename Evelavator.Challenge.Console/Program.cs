using Elevator.Challenge.Console.Helpers;
using Elevator.Challenge.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elevator.Challenge.Console;

internal class Program
{
    private static async Task Main()
    {
        var serviceProvider = ConfigureServices();
        var userInteractionService = serviceProvider.GetRequiredService<UserInteractionService>();
        await userInteractionService.RunAsync();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        services.AddSingleton(configuration);

        // Register services
        services.AddLogging(configure => configure.AddConsole());
        services.AddTransient<IPrintHelper, PrintHelper>();
        services.AddTransient<IUserInputHelper, UserInputHelper>();
        services.AddScoped<IElevatorService, ElevatorService>();
        services.AddSingleton<IBuildingService, BuildingService>();
        services.AddSingleton<UserInteractionService>();

        return services.BuildServiceProvider();
    }
}