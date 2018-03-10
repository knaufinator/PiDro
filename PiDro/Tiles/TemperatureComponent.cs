using Pidro.Properties;
using Pidro.Tools;
using MathNet.Numerics;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;


namespace Pidro.Tiles
{
    public class TemperatureComponent : ComponentInterface
    {
        TemperatureTile temperatureTile = new TemperatureTile();
        Timer updateTimer;
        String address;// = "28-01161b0ab2ee";
        String name;

        public TemperatureComponent(String Name,String W1Address)
        {
            address = W1Address;
            name = Name;

            temperatureTile.nameLabel.Text = Name;

            //update the clock readout one a sec
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            temperatureTile.Set(GetTemp().ToString("N1"));
        }

        private Double GetTemp()
        {
            Double result = 0.0;

            try
            {
                DirectoryInfo devicesDir = new DirectoryInfo("/sys/bus/w1/devices");
                
                foreach (var deviceDir in devicesDir.EnumerateDirectories(address))
                {
                    var w1slavetext =
                        deviceDir.GetFiles("w1_slave").FirstOrDefault().OpenText().ReadToEnd();
                    string temptext =
                        w1slavetext.Split(new string[] { "t=" }, StringSplitOptions.RemoveEmptyEntries)[1];

                    return ConvertTemp.ConvertCelsiusToFahrenheit(double.Parse(temptext) / 1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);   
            }

            return result;
        }
     
        public System.Windows.Forms.Control GetTile()
        {
            return temperatureTile;
        }
    }    
}
