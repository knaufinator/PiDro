using Pidro.Properties;
using MathNet.Numerics;
using System;
using System.Windows.Forms;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Pidro.Tools;

namespace Pidro.Tiles
{
    public class PressureComponent : ComponentInterface
    {
        PressureTile pressureTile = new PressureTile();

        Timer updateTimer;
        ADConverter aDConverter;

        AppSettings settings = AppSettings.Instance;
        int sensorId = 1;
        double pressure110;
        double pressure80;

        String pressureNode = "Pressure";
        String pressure110Setting = "Pressure110";
        String pressure80Setting = "Pressure80";

        public PressureComponent(ADConverter aDConverter)
        {
            
            LoadSettings();
            this.aDConverter = aDConverter;
            pressureTile.button1.Click += Button1_Click;
    
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

                result = ADConverter.Interpolate(x,y,aDConverter.GetADVoltage(sensorId));
            }
          
            return result;
        }

        public void calibratePressureAuto()
        {
           
            Double phV = aDConverter.GetADVoltage(sensorId);
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
