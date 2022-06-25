using System;

using Microsoft.Extensions.Logging;

namespace nanoFramework.DiApplication
{
    internal class Application
    {
        private readonly ILogger _logger;
        private readonly IHardwareService _hardware;
        private readonly IServiceProvider _provider;

        public Application(IServiceProvider provider, IHardwareService hardware, ILoggerFactory loggerFactory)
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
}