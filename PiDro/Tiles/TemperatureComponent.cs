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
using System.Reactive.Linq;
using Pidro.Settings;

namespace Pidro.Tiles
{
    public class TemperatureComponent : ComponentInterface
    {
        TemperatureTile temperatureTile = new TemperatureTile();
        Timer updateTimer;
        String address;// = "28-01161b0ab2ee";
        String name;

        static Object locker = new Object();
        private W1TempItem item;
        
        public TemperatureComponent(W1TempItem item)
        {
            this.item = item;
            address = item.Address;
            temperatureTile.nameLabel.Text = item.name;

            //update the ph value, every 2 seconds,
            IObservable<long> timer = Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(2000));
            IDisposable disposable =
                timer.Subscribe(x => {
                    try
                    {
                        Update();
                    }
                    catch (Exception ex)
                    {
                    }
                });
        }

        private void Update()
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
