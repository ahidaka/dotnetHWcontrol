using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace co2
{
    class Program
    {
        public static void Main(string[] args)
        {
            UARTSensor uart;

            Console.WriteLine("Hello CO2");
            uart = new UARTSensor();
            uart.Initialize();

            while(true) 
            {
                Thread.Sleep(1000);
                Console.WriteLine("CO2:" +
                    uart.ReadData().ToString("F0") + " ppm");
            }
        }
    }

    public class UARTSensor
    {
        private SerialPort serialDev;
        double co2data = 0.0D;

        public void Initialize()
        {
            serialDev = new SerialPort();
            if (serialDev == null)
            {
                Console.WriteLine("serialDev is null");
                return;
            }
            serialDev.PortName = "/dev/ttyS0";
            serialDev.WriteTimeout = 1500;
            serialDev.ReadTimeout = 1500;
            serialDev.BaudRate = 9600;
            serialDev.Parity = (Parity)Enum.Parse(typeof(Parity), "None", true);
            serialDev.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One", true);
            serialDev.DataBits = 8;
            serialDev.Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", false);
        }

        private int Co2Format(string s)
        {
            //string s = " Z 00483 z 00486\r";
            //    this will pickup   ^^^^^
            int value = 0;
            int zPosition = s.IndexOf('z');
            if (zPosition > 0 && zPosition <= 9)
            {
                zPosition += 2;
                string target = s.Substring(zPosition, 5);
                value = int.Parse(target);
            }
            return value;
        }

        public int ReadData()
        {
            string readData = serialDev.ReadLine();
            return Co2Format(readData);
        }
    }
}
