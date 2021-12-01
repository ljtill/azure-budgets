using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;

using Microsoft.Extensions.Logging;

namespace Microsoft.AppInnovation.Budgets
{
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
}