using HydroTest.Properties;
using MathNet.Numerics;
using System;
using System.Windows.Forms;
using System.Xml;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;


namespace HydroTest.Tiles
{
    public class TemperatureComponent : ComponentInterface
    {
        TemperatureTile pressureTile = new TemperatureTile();

      //  Timer updateTimer;
        //I2CDevice myDevice;

        public TemperatureComponent( )
        {

     
            try
            {
              //  myDevice = Pi.I2C.AddDevice(0x48);          
            }
            catch(Exception e)
            {
            }
    
            //update the clock readout one a sec
            //updateTimer = new Timer();
            //updateTimer.Interval = 1000;
            //updateTimer.Tick += UpdateTimer_Tick;
            //updateTimer.Start();
        }
     
        public System.Windows.Forms.Control GetTile()
        {
            return pressureTile;
        }
    }    
}
