using Microsoft.Extensions.Logging;

using nanoFramework.Logging.Debug;
using nanoFramework.DependencyInjection;

namespace DependencyInjection
{
    public class Program
    {
        public static void Main()
        {
            var serviceProvider = ConfigureServices();
            var application = (Application)serviceProvider.GetRequiredService(typeof(Application));
            
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