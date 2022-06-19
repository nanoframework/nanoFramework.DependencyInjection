using System;
using System.Threading;
using System.Device.Gpio;

using Microsoft.Extensions.Logging;

namespace DI
{
    internal class HardwareService : IHardwareService, IDisposable
    {
        private Thread _thread;
        private readonly ILogger _logger;
        private readonly GpioController _gpioController;

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