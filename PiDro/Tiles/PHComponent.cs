using HydroTest.Properties;
using MathNet.Numerics;
using System;
using System.Windows.Forms;
using System.Xml;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;


namespace HydroTest.Tiles
{
    public class PHComponent : ComponentInterface
    {
        PHTile phTile = new PHTile();

        Timer updateTimer;
        I2CDevice myDevice;
        Timer phUp = new Timer();
        Timer phDown = new Timer();
        AppSettings settings = AppSettings.Instance;
        double ph4;
        double ph7;

        String phNode = "PH";
        String ph7Setting = "PH7";
        String ph4Setting = "PH4";

        public PHComponent( )
        {
            LoadSetting();          

            phTile.button1.Click += Button1_Click;
            phTile.button2.Click += Up_Click;
            phTile.button2.Click += Down_Click;

            phUp.Interval = 2000;
            phUp.Tick += PH_pump_up;

            phDown.Interval = 2000;
            phDown.Tick += PH_pump_down;

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
            calibratePHAuto();
        }

        private void LoadSetting()
        {
            Double.TryParse(settings.GetSetting(phNode, ph4Setting, "1.0"), out ph4);
            Double.TryParse(settings.GetSetting(phNode, ph7Setting, "2.0"), out ph7);
        }

        private void SaveSettings()
        {
            settings.SaveSetting(phNode, ph4Setting, ph4.ToString());
            settings.SaveSetting(phNode, ph4Setting, ph7.ToString());
        }
        
        private void Cal_Click(object sender, EventArgs e)
        {
            calibratePHAuto();
        }

        public Double getPH()
        {
            double result = 0.0;

            if (ph4 > 0 & ph7 > 0)
            {
                double[] x = { ph4, ph7 };
                double[] y = { 4, 7 };

                try
                {                            
                    Tuple<double, double> p = Fit.Line(x, y);
                    double c = p.Item1; // == 10; intercept
                    double m = p.Item2; // == 0.5; slope

                    //y = mx + c;
                    result = m * this.getPHVoltage() + c;
                }
                catch (Exception e)
                {
                    String test = "";
                }
            }
          
            return result;
        }

        public void calibratePHAuto()
        {
            Double phV = getPHVoltage();
            Double vCutoff = 1.5;

            if (phV >= vCutoff)
            {
                ph7 = phV;
                settings.SaveSetting(phNode, ph7Setting, phV.ToString());
            }
            else
            {
                ph4 = phV;
                settings.SaveSetting(phNode, ph4Setting, phV.ToString());
            }
        }

        public Double getPHVoltage()
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

        private void Down_Click(object sender, EventArgs e)
        {
            try
            {
                Pi.Gpio.Pin13.Write(true);
            }
            catch (Exception)
            {
            }

            phDown.Start();
        }
        
        private void Up_Click(object sender, EventArgs e)
        {
            try
            {
                Pi.Gpio.Pin26.Write(true);
            }
            catch (Exception)
            {
            }

            phUp.Start();
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
            phTile.set(getPH().ToString("N1"));
        }
    
        public System.Windows.Forms.Control GetTile()
        {
            return phTile;
        }
    }    
}
