using Microsoft.Extensions.DependencyInjection;

namespace Budgets;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            // TODO: Patch worker logging
            .ConfigureLogging(c => c.SetMinimumLevel(LogLevel.Debug))
            .Build();

        host.Run();
    }
}