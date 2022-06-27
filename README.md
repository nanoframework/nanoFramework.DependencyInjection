[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.DependencyInjection&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.DependencyInjection) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.DependencyInjection&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.DependencyInjection) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.DependencyInjection.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.DependencyInjection/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

# Welcome to the .NET nanoFramework Dependency Injection Library repository

Provides Dependency Injection (DI) for Inversion of Control (IoC) between classes and their dependencies built for .NET nanoFramework.

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.DependencyInjection | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.DependencyInjection/_apis/build/status/nanoFramework.DependencyInjection?branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.DependencyInjection/_build/latest?definitionId=95&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.DependencyInjection.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.DependencyInjection/) |

## Samples

[Dependency Injection Sample](https://github.com/nanoframework/Samples/tree/main/samples/DependencyInjection)

## Dependency Injection Container

A Dependency Injection (DI) Container provides functionality and automates many of the tasks involved in Object Composition, Interception, and Lifetime Management. It's an engine that resolves and manages object graphs. These DI Containers depend on the static information compiled into all classes. Then using reflection they can analyze the requested class and figure out which Dependencies are required.

This API mirrors as close as possible the official .NET 
[DependencyInjection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection). Exceptions are mainly derived from the lack of generics support in .NET nanoFramework.

## Usage

### Service Collection

Creating a dependency injection container required three basic components.

 * Object Composition - A object composition defining a set of objects to create and couple.
 * Registering Services - Define an instance of the ServiceCollection and register the object composition with a specific service lifetime.
 * Service Provider - Creating a service provider to retrieve the object. 

### Object Composition

Define an object composition to create and couple.

```csharp
public class RootObject
{
    public int One { get; }
    
    public string Two { get; }

    public ServiceObject ServiceObject { get; protected set; }

    public RootObject(ServiceObject serviceObject)
    {
        ServiceObject = serviceObject;
    }

    // constructor with the most parameters will be used for activation
    public RootObject(ServiceObject serviceObject, int one, string two)
    {
        ServiceObject = serviceObject;
        One = one;
        Two = two;
    }
}

public class ServiceObject
{
    public string Three { get; set; }
}
```

### Registering Services

Create a Service Collection and register singleton or transient type services to the collection.

```csharp
var serviceProvider = new ServiceCollection()
    .AddSingleton(typeof(ServiceObject))
    .AddSingleton(typeof(RootObject))
    .BuildServiceProvider();
```
### Service Provider

Create a Service Provider to access or update an object.

```csharp
var service = (RootObject)serviceProvider.GetService(typeof(RootObject));
service.ServiceObject.Three = "3";
```

## Activator Utilities

An instance of an object can be created by calling its constructor with any dependencies resolved through the service provider. Automatically instantiate a type with constructor arguments provided from an IServiceProvider without having to register the type with the DI Container.

```csharp
var instance = (RootObject)ActivatorUtilities.CreateInstance(
                        serviceProvider, typeof(RootObject), 1, "2"
                    );

Debug.WriteLine($"One: {instance.One}");
Debug.WriteLine($"Two: {instance.Two}");
Debug.WriteLine($"Three: {instance.ServiceObject.Three}");
Debug.WriteLine($"Name: {instance.ServiceObject.GetType().Name}");
```

###  Validate On Build 

A check is performed to ensure that all services registered with the container can actually be created. This can be particularly useful during development to fail fast and allow developers to fix the issue. Validate on build is configured false by default.

```csharp
var serviceProvider = new ServiceCollection()
    .AddSingleton(typeof(IServiceObject), typeof(ServiceObject))
    .BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true });
```

## Example Application Container

```csharp
using System;
using System.Device.Gpio;
using System.Threading;

using nanoFramework.Logging.Debug;
using nanoFramework.DependencyInjection;

using Microsoft.Extensions.Logging;

nanoFramework.DiApplication
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

    internal class Application
    {
        private readonly ILogger _logger;
        private readonly IHardwareService _hardware;
        private readonly IServiceProvider _provider;

        public Application(
            IServiceProvider provider,
            IHardwareService hardware, 
            ILoggerFactory loggerFactory)
        {
            _provider = provider;
            _hardware = hardware;
            _logger = loggerFactory.CreateLogger(nameof(Application));

            _logger.LogInformation("Initializing application...");
        }

        public void Run()
        {
            var ledPin = 23; // Set pin number to blink 15=ESP32; 23=STM32

            _logger.LogInformation($"Started blinking led on pin {ledPin}.");
            _hardware.StartBlinking(ledPin);
        }
    }

    internal interface IHardwareService
    {
        public void StartBlinking(int ledPin) { }
    }

    internal class HardwareService : IHardwareService, IDisposable
    {
        private Thread _thread;
        private readonly ILogger _logger;
        private readonly GpioController _gpioController;

        public HardwareService()
        {
            _gpioController = new GpioController();

            var loggerFactory = new DebugLoggerFactory();
            _logger = loggerFactory.CreateLogger(nameof(HardwareService));
        }

        public HardwareService(ILoggerFactory loggerFactory)
        {
            _gpioController = new GpioController();
            _logger = loggerFactory.CreateLogger(nameof(HardwareService));
        }

        public void StartBlinking(int ledPin)
        {
            GpioPin led = _gpioController.OpenPin(ledPin, PinMode.Output);
            led.Write(PinValue.Low);

            _thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(2000);

                    led.Write(PinValue.High);
                    _logger.LogInformation("Led status: on");

                    Thread.Sleep(2000);

                    led.Write(PinValue.Low);
                    _logger.LogInformation("Led status: off");
                }
            });

            _thread.Start();
        }

        public void Dispose()
        {
            _gpioController.Dispose();
        }
    }
}
```

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).