// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Spi;
using System.Threading;

namespace Lm75b.Samples
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            SPISensor spi = new SPISensor();
            spi.Initialize();

            {
                while (true)
                {
                    // read temperature
                    Console.WriteLine($"Temperature: {sensor.Temperature.Celsius} ℃");
                    Console.WriteLine();

                    Thread.Sleep(1000);
                }
            }
        }
    }

    public class SPISensor
    {
        private const byte CHIP_SELECT = 0;
        private const byte RW_BIT = 0x80;
        private const byte MB_BIT = 0x40;
        private SpiAdapter spiDev;
        
        public async void Initialize()
        {
            SpiConnectionSettings settings = new SpiConnectionSettings(CHIP_SELECT);
            settings.ClockFrequency = 5000000;
            settings.Mode = SpiMode.Mode3; // CPOL = 1, CPHA = 1         
            IReadOnlyList<DeviceInformation> dev
                = await DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector());
            spiDev = new SpiDevice.FromIdAsync(dev[0].Id, settings);
            if (spiDev == null)
            {
                Console.WriteLine("spiDev is null!");
                return;
            }

            byte[] WriteBuf_Addr = new byte[] { 0x0F | RW_BIT | MB_BIT, 0 };
            byte[] ReadBuffer = new byte[2];
            spiDev.TransferFullDuplex(WriteBuf_Addr, ReadBuffer);

            if (ReadBuffer[1] != 0xbd)
            {
                Console.WriteLine("Who am I failed!");
                return;
            }

            byte[] WriteData = new byte[] { 0x20, 0x90 }; //Power on
            spiDev.Write(WriteData);
        }
        public double ReadData()
        {
            byte[] ReadData0 = new byte[2];
            byte[] ReadData1 = new byte[2];
            byte[] ReadData2 = new byte[2];
            byte[] WriteData = new byte[] { 0x28 | RW_BIT | MB_BIT, 0 };
            spiDev.TransferFullDuplex(WriteData, ReadData0);

            WriteData = new byte[] { 0x29 | RW_BIT | MB_BIT, 0 };
            spiDev.TransferFullDuplex(WriteData, ReadData1);

            WriteData = new byte[] { 0x2A | RW_BIT | MB_BIT, 0 };
            spiDev.TransferFullDuplex(WriteData, ReadData2);

            uint barometer = (uint)(ReadData2[1] << 16);
            barometer |= (uint)(ReadData1[1] << 8);
            barometer |= ReadData0[1];

            return barometer / 4096;
        }
    }
}
