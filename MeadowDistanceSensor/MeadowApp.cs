using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Distance;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Distance;
using System;
using System.Threading;


namespace MeadowDistanceSensor
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        GraphicsLibrary canvas;
        Vl53l0x distanceSensor;

        public MeadowApp()
        {
            Initialize();

            Console.WriteLine("Start sampling");
            distanceSensor.Updated += DistanceUpdated;
            distanceSensor.StartUpdating(100);
        }

        void Initialize()
        {
            Console.WriteLine("Initializing hardware...");

            Console.WriteLine("Initialize SPI bus");

            var config = new SpiClockConfiguration(12000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            Console.WriteLine("Initialize display driver instance");

            St7789 display = new St7789(device: Device, spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240);

            Console.WriteLine("Create graphics library");

            canvas = new GraphicsLibrary(display)
            {
                CurrentFont = new Font12x20()
            };
            canvas.DrawRectangle(0, 0, 240, 240, Color.Black, true);
            canvas.Clear(true);

            Console.WriteLine("Initialize distance sensor");
            var i2cBus = Device.CreateI2cBus(I2cBusSpeed.FastPlus);
            distanceSensor = new Vl53l0x(Device, i2cBus);
        }

        private void DistanceUpdated(object sender, DistanceConditionChangeResult e)
        {
            if (e.New == null || e.New.Distance == null)
            {
                return;
            }

            Console.WriteLine($"{e.New.Distance.Value}mm");

            canvas.DrawRectangle(70, 90, 144, 40, Color.Black, true);
            canvas.DrawText(70, 90, $"{e.New.Distance.Value}mm", Color.White, GraphicsLibrary.ScaleFactor.X2);
            canvas.Show();
        }
    }
}
