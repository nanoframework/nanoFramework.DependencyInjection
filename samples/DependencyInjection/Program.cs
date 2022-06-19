using nanoFramework.Logging.Debug;
using nanoFramework.DependencyInjection;

using Microsoft.Extensions.Logging;

namespace DI
{
    public class Program
    {
        public static void Main()
        {
            var services = ConfigureServices();
            var application = (Application)services.GetRequiredService(typeof(Application));

            application.Run();

        }

        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(typeof(Application))
                .AddSingleton(typeof(IHardwareService), typeof(HardwareService))
                .AddSingleton(typeof(ILoggerFactory), typeof(DebugLoggerFactory))
                .BuildServiceProvider();
        }
    }
}