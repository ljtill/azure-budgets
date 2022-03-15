using Microsoft.Extensions.DependencyInjection;

namespace Budgets;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureLogging(c => c.SetMinimumLevel(LogLevel.Debug))
            .Build();

        host.Run();
    }
}