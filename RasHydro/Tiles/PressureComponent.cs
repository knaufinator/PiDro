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
        double pressure70;

        String pressureNode = "Pressure";
        String pressure110Setting = "Pressure110";
        String pressure70Setting = "Pressure70";

        public PressureComponent( )
        {
            //LoadSetting();          
          

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

        private void Button1_Click(object sender, EventArgs e)
        {
            calibratePressureAuto();
        }

        private void SaveSettings()
        {
            settings.SaveSetting(pressureNode, pressure70Setting, pressure70.ToString());
            settings.SaveSetting(pressureNode, pressure110Setting, pressure110.ToString());
        }
        
        public Double getPressure()
        {
            double result = 0.0;

            if (pressure70 > 0 & pressure110 > 0)
            {
                double[] x = { pressure70, pressure110 };
                double[] y = { 70, 110 };

                try
                {                            
                    Tuple<double, double> p = Fit.Line(x, y);
                    double c = p.Item1; // == 10; intercept
                    double m = p.Item2; // == 0.5; slope

                    //y = mx + c;
                    result = m * this.getPressureVoltage() + c;
                }
                catch (Exception e)
                {
                    String test = "";
                }
            }
          
            return result;
        }

        public void calibratePressureAuto()
        {
            Double phV = getPressureVoltage();
            Double vCutoff = 1.5;

            if (phV >= vCutoff)
            {
                pressure110 = phV;
                settings.SaveSetting(pressureNode, pressure110Setting, phV.ToString());
            }
            else
            {
                pressure70 = phV;
                settings.SaveSetting(pressureNode, pressure70Setting, phV.ToString());
            }
        }

     
        public Double getPressureVoltage()
        {
            double measuredVoltage = 5.0;
            double adSteps = 255;
            double result = 0.0;
            byte buffer;

            try
            {
                myDevice.Write((byte)(0x40 | (0 & 3)));
                buffer = (byte)myDevice.Read();
                
                short unsignedValue = (short)((short)0x00FF & buffer);
                result = unsignedValue * measuredVoltage / adSteps;
            }
            catch (Exception e)
            {
              
            }

            return result;
        }

        private void PH_pump_up(object sender, EventArgs e)
        {
            //turn off pump
            try
            {
                Pi.Gpio.Pin26.Write(false);
            }
            catch (Exception)
            {
            }
        }

        private void PH_pump_down(object sender, EventArgs e)
        {
            //turn off pump
            try
            {
                Pi.Gpio.Pin13.Write(false);
            }
            catch (Exception)
            {
            }

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
