using System;
using System.Threading;
using System.Device.Gpio;

namespace blinky
{
    class Program
    {
        static void Main(string[] args)
        {
            const int pin = 24;
            using (var controller = new GpioController())
            {
                controller.OpenPin(pin, PinMode.Output);
                while (true)
                {
                    Console.WriteLine("Blink!");
                    controller.Write(pin, PinValue.High);
                    Thread.Sleep(200);
                    controller.Write(pin, PinValue.Low);
                    Thread.Sleep(800);
                }
            }
        }
    }
}
