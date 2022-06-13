using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using nanoFramework.Logging.Debug;
using nanoFramework.DependencyInjection;

namespace DependencyInjection
{
    public class Program
    {
        public static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(ILoggerFactory), typeof(DebugLoggerFactory))
                .AddSingleton(typeof(IFooService), typeof(FooService))
                .AddSingleton(typeof(IBarService), typeof(BarService))
                .AddSingleton(typeof(DisposableService))
                .BuildServiceProvider();

            var loggerFactory = (DebugLoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));
            var logger = loggerFactory.CreateLogger(nameof(Program));
            logger.LogInformation("Starting application...");

            var service = (DisposableService)serviceProvider.GetService(typeof(DisposableService));
            logger.LogInformation(service.ToString());

            //do some actual work here
            var bar = (BarService)serviceProvider.GetService(typeof(IBarService));
            bar.DoSomeRealWork();

            logger.LogInformation("All done!");

            serviceProvider.Dispose();

            Thread.Sleep(Timeout.Infinite);
        }


        public interface IStructFakeService
        {
        }

        public struct StructFakeService : IStructFakeService
        {
            public StructFakeService(IServiceProvider serviceProvider)
            {
            }
        }

        public interface IFooService
        {
            void DoThing(int number);
        }
        public interface IBarService
        {
            void DoSomeRealWork();
        }
        
        public class BarService : IBarService
        {
            private readonly IFooService _fooService;

            public BarService(IFooService fooService)
            {
                _fooService = fooService;
            }

            public void DoSomeRealWork()
            {
                for (int i = 0; i < 10; i++)
                {
                    _fooService.DoThing(i);
                }
            }
        }
        public class FooService : IFooService
        {
            private ILogger _logger;

            public FooService(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger(nameof(FooService));
            }

            public void DoThing(int number)
            {
                _logger.LogInformation($"Doing the thing {number}");
            }
        }
        public class DisposableService : IDisposable
        {
            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~DisposableService()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public override string ToString()
            {
                return "Hello from disposable service";
            }
        }
    }
}

//var serviceProvider = new ServiceCollection()
//    .AddTransient(typeof(IStructFakeService), typeof(StructFakeService))
//    .BuildServiceProvider();

//var service1 = serviceProvider.GetService(typeof(IStructFakeService));
//var service2 = serviceProvider.GetService(typeof(IStructFakeService));