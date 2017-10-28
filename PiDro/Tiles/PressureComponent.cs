using HydroTest.Properties;
using MathNet.Numerics;
using System;
using System.Windows.Forms;
using System.Xml;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;


namespace HydroTest.Tiles
{
    public class PressureComponent : ComponentInterface
    {
        PressureTile pressureTile = new PressureTile();

        Timer updateTimer;
        I2CDevice myDevice;

        AppSettings settings = AppSettings.Instance;

        double pressure110;
        double pressure80;

        String pressureNode = "Pressure";
        String pressure110Setting = "Pressure110";
        String pressure80Setting = "Pressure80";

        public PressureComponent( )
        {
            LoadSettings();                    
            pressureTile.button1.Click += Button1_Click;

            try
            {
                myDevice = Pi.I2C.AddDevice(0x48);          
            }
            catch(Exception e)
            {
            }
    
            //update the clock readout one a sec
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void LoadSettings()
        {
            Double.TryParse(settings.GetSetting(pressureNode, pressure80Setting, "1.0"), out pressure80);
            Double.TryParse(settings.GetSetting(pressureNode, pressure110Setting, "3.0"), out pressure110);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            calibratePressureAuto();
        }  
        
        public Double getPressure()
        {
            double result = 0.0;

            if (pressure80 > 0 & pressure110 > 0)
            {
                double[] x = { pressure80, pressure110 };
                double[] y = { 80, 110 };
                
                try
                {                            
                    Tuple<double, double> p = Fit.Line(x, y);
                    double c = p.Item1;
                    double m = p.Item2;

                    //y = mx + c;
                    result = m * this.getPressureVoltage() + c;
                }
                catch (Exception e)
                {                    
                }
            }
          
            return result;
        }

        public void calibratePressureAuto()
        {
           
            Double phV = getPressureVoltage();
            Double vCutoff = 2.8;

            Console.WriteLine("Calibrating Pressure: " + phV.ToString());

            if (phV >= vCutoff)
            {
                pressure110 = phV;
                settings.SaveSetting(pressureNode, pressure110Setting, phV.ToString());
            }
            else
            {
                pressure80 = phV;
                settings.SaveSetting(pressureNode, pressure80Setting, phV.ToString());
            }
        }
     
        public Double getPressureVoltage()
        {
            double measuredVoltage = 5.0;
            double adSteps = 255;
            double result = 0.0;
            byte buffer;
            int sensorId = 1;
            try
            {
                myDevice.Write((byte)(0x40 | (sensorId & 3)));
                buffer = (byte)myDevice.Read();
                buffer = (byte)myDevice.Read();
                short unsignedValue = (short)((short)0x00FF & buffer);
                result = unsignedValue * measuredVoltage / adSteps;
            }
            catch (Exception e)
            {
              
            }

            return result;
        }
        
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            pressureTile.set(getPressure().ToString("N1"));
        }
    
        public System.Windows.Forms.Control GetTile()
        {
            return pressureTile;
        }
    }    
}
