using System;
using System.Device.I2c;

namespace sht31
{
    class Program
    {
        const int BUS_ID = 1;
        const int DEVICE_ADDRESS = 0x45;

        static void Main(string[] args)
        {
            var settings = new I2cConnectionSettings(BUS_ID, DEVICE_ADDRESS);
            var device = I2cDevice.Create(settings);

            ReadOnlySpan<byte> writeBuf = stackalloc byte[] { 0x2c, 0x10 };
            Span<byte> readBuf = stackalloc byte[6];
            device.Write(writeBuf);
            device.Read(readBuf);
        }
    }
}
