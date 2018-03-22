using Pidro.Properties;
using MathNet.Numerics;
using System;
using System.Windows.Forms;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Pidro.Tools;
using Pidro.Settings;

namespace Pidro.Tiles
{
    public class PressureComponent : ComponentInterface
    {
        PressureTile pressureTile = new PressureTile();
        private Guid ID;

        Timer updateTimer;
        ADConverter aDConverter;

        AppSettings settings = AppSettings.Instance;
        int sensorId = 1;
        double pressure110 = 1;
        double pressure80 = 3;

        public PressureComponent(Settings.PressureItem item)
        {
            ID = item.ID;
            pressureTile.button1.Click += Button1_Click;
            sensorId = item.adcAddress;
            pressure110 = item.pressure110;
            pressure80 = item.pressure80;

            try
            {
                aDConverter = new ADConverter(item.adcAddress);
            }
            catch (Exception e)
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
        
        public Double getPressure()
        {
            double result = 0.0;
            
            if (pressure80 > 0 & pressure110 > 0)
            {
                double[] x = { pressure80, pressure110 };
                double[] y = { 80, 110 };



                result = ADConverter.Interpolate(x,y,aDConverter.GetADVoltage(sensorId));
            }
          
            return result;
        }

        public void calibratePressureAuto()
        {
            PressureItem setting = (PressureItem)settings.CurrentSettings.GetComponent(ID);

            Double phV = aDConverter.GetADVoltage(sensorId);
            Double vCutoff = 2.8;

            Console.WriteLine("Calibrating Pressure: " + phV.ToString());

            if (phV >= vCutoff)
            {
                if (MessageBox.Show("110 PSI ? @ "  + phV.ToString()+"V", "Calibrate 110PSI", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    pressure110 = phV;
                    setting.pressure110 = phV;
                    settings.Save();
                }
            }
            else
            {
                if (MessageBox.Show("80 PSI ? @ " + phV.ToString() + "V", "Calibrate 80PSI", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    pressure80 = phV;
                    setting.pressure80 = phV;
                    settings.Save();
                }
                
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
