using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace Pidro
{
    public sealed class ADConverter
    {
        public I2CDevice myDevice;
        private static ADConverter instance = null;
        private static readonly object padlock = new object();

        public static ADConverter Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ADConverter();
                    }
                    return instance;
                }
            }
        }


        ADConverter()
        {
            try
            {
               
            }
            catch (Exception e)
            {
               
            }
        }


        public Double GetADVoltage(int sensorId)
        {
            double measuredVoltage = 5.0;
            double adSteps = 255;
            double result = 0.0;
            byte buffer;

            try
            {
                myDevice.Write((byte)(0x40 | (sensorId & 3)));

                //this has the goods onthe second read, without this, you get bleedover to other sensor reads.
                myDevice.Read();
                buffer = (byte)myDevice.Read();

                short unsignedValue = (short)((short)0x00FF & buffer);
                result = unsignedValue * measuredVoltage / adSteps;
            }
            catch (Exception e)
            {
                Console.WriteLine("AD read error: " + e.Message);
            }

            return result;
        }

    }

}
