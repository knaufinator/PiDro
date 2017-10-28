using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Pidro.Tools
{
    public class ADConverter
    {
        public I2CDevice myDevice;

        public ADConverter(int deviceID)
        {
            try
            {
                myDevice = Pi.I2C.AddDevice(deviceID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Ad Converter init: "+ e.Message );
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
