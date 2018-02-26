using Pidro.Properties;
using MathNet.Numerics;
using System;
using System.Windows.Forms;
using System.Xml;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using Pidro.Tools;
using System.Reactive.Linq;
using System.Windows.Threading;
using System.Reactive.Subjects;
using System.Text;

namespace Pidro.Tiles
{
    public class PHComponent : ComponentInterface
    {
        PHTile phTile = new PHTile();
        int sensorId = 0;
        I2CDevice ezoDevice;

        IDisposable subscription;

        ADConverter aDConverter;
        AppSettings settings = AppSettings.Instance;

        Boolean autoOn = false;

        int pumpOnTimeSeconds = 2;
        int pumpAutoOnTimeSeconds = 1;

        double ph4;
        double ph7;

        double targetPH = 5.7;
        double phTolerance = .2;

        String phNode = "PH";
        String ph7Setting = "PH7";
        String ph4Setting = "PH4";
        
        public PHComponent()
        {
            LoadSettings();

            try
            {
               // Pi.Gpio.Pins[]

              //Pi.Gpio.Pin25.PinMode = GpioPinDriveMode.Output;//ph up
              //Pi.Gpio.Pin23.PinMode = GpioPinDriveMode.Output;//ph down
            }
            catch
            {
            }

            //   phTile.button1.Click += Cal_Click;
            //  phTile.button2.Click += Up_Click;
            // phTile.button3.Click += Down_Click;
            //phTile.button4.Click += Auto_Click;

            Observable
           .Interval(TimeSpan.FromSeconds(1))
           .ObserveOn(Dispatcher.CurrentDispatcher)
           .Subscribe(
               x =>
               {
                   Update();
               });
        }

        public PHComponent(ADConverter aDConverter) : base()
        {
            this.aDConverter = aDConverter;
            
           
        }

        public PHComponent(int deviceID) : base()
        {        
            try
            {
                ezoDevice = Pi.I2C.AddDevice(deviceID);

            }
            catch
            {
            }
        }

        private void Auto_Click(object sender, EventArgs e)
        {        
            if (autoOn == false)
            {
              //  phTile.label2.Text = "ON";
                autoOn = true;
                //sample every 30 min, if over tartget, apply correct dose up/down, once 

                subscription = Observable
                   .Interval(TimeSpan.FromSeconds(6))
                   .ObserveOn(Dispatcher.CurrentDispatcher)
                   .Subscribe(
                       x =>
                       {
                           try
                           {
                               CheckPHAndDose();
                           }
                           catch (Exception)
                           {
                           }
                       });
            }
            else
            {
               // phTile.label2.Text = "OFF";
                autoOn = false;

                if (subscription != null)
                    subscription.Dispose();
            }
        }

        BehaviorSubject<bool> switches = new BehaviorSubject<bool>(false);
        
        private void CheckPHAndDose()
        {
            Double ph = GetPH();
            if (ph > targetPH + phTolerance)
            {
                //dose ph down,
                PHDownDose(pumpAutoOnTimeSeconds);
            }
            else if (ph < targetPH - phTolerance)
            {
                PHUpDose(pumpAutoOnTimeSeconds);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            calibratePHAuto();
        }

        private void LoadSettings()
        {
            Double.TryParse(settings.GetSetting(phNode, ph4Setting, "1.0"), out ph4);
            Double.TryParse(settings.GetSetting(phNode, ph7Setting, "2.0"), out ph7);
        }
        
        private void Cal_Click(object sender, EventArgs e)
        {
            calibratePHAuto();
        }

        public Double GetPH()
        {
            double result = 0.0;

            if (ph4 > 0 & ph7 > 0)
            {
                double[] x = { ph4, ph7 };
                double[] y = { 4, 7 };

                result = ADConverter.Interpolate(x, y, aDConverter.GetADVoltage(sensorId));
            }
          
            return result;
        }

        public void calibratePHAuto()
        {
            Double phV = aDConverter.GetADVoltage(sensorId);
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

        private void TryPinWrite(GpioPin pin, bool level)
        {
            try
            {
                pin.Write(level);
            }
            catch (Exception)
            {
            }
        }

        private void Up_Click(object sender, EventArgs e)
        {
            PHUpDose(pumpOnTimeSeconds);
        }
        
        private void Down_Click(object sender, EventArgs e)
        {
            PHDownDose(pumpOnTimeSeconds);
        }

        private void PHDownDose(int seconds)
        {
            RunPump(Pi.Gpio.Pin23, seconds);
        }

        private void PHUpDose(int seconds)
        {
            RunPump(Pi.Gpio.Pin25, seconds);
        }

        private void RunPump(GpioPin pin, int onTime)
        {
            TryPinWrite(pin, true);

            Observable
              .Timer(TimeSpan.FromSeconds(onTime))
              .Subscribe(
                  x =>
                  {
                      TryPinWrite(pin, false);
                  });
        }
        
        private void Update()
        {
            //phTile.phValue.Text = GetPH().ToString("N1");
            ezoDevice.Write(Encoding.ASCII.GetBytes("r"));
            
            Observable
            .Timer(TimeSpan.FromMilliseconds(900))
            .Subscribe(
            x =>
            {
                //wait some time, then read
                byte d = ezoDevice.Read();
                phTile.set(d.ToString());
            });
        }
    
        public System.Windows.Forms.Control GetTile()
        {
            return null;
        }
    }    
}
