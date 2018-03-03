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
using System.Text;
using System.Collections.Generic;
using System.Reactive.Subjects;
using RaspberrySharp.IO.InterIntegratedCircuit;
using RaspberrySharp.IO.GeneralPurpose;

namespace Pidro.Tiles
{
    public class PHComponent : ComponentInterface
    {
        PHTile phTile = new PHTile();
        int sensorId = 0;
       // I2CDevice ezoDevice;

        IDisposable subscription;

        ADConverter aDConverter;
        AppSettings settings = AppSettings.Instance;

        Boolean autoOn = false;

        int pumpOnTimeSeconds = 2;
        int pumpAutoOnTimeSeconds = 1;

        private static I2cDriver driver;
        private static I2cDeviceConnection i2cConnection;

        double ph4;
        double ph7;

        double targetPH = 5.7;
        double phTolerance = .2;
        double PH = 0;

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

            IObservable<long> timer = Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(2000));

            IDisposable disposable =
                timer.Subscribe(x =>
                {
                    
                    try
                    {
                        Update();
                    }
                    catch (Exception ex)
                    {
                    }
                });


        }

        public PHComponent(ADConverter aDConverter) : this()
        {
            this.aDConverter = aDConverter;
        }

        public PHComponent(int deviceID) : this()
        {        
            try
            {
                //ezoDevice = Pi.I2C.AddDevice(deviceID);
                driver = new I2cDriver(ProcessorPin.Gpio02, ProcessorPin.Gpio03);
                i2cConnection = driver.Connect(0x63);
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
         // as
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
            try
            {
                i2cConnection.Write(Encoding.ASCII.GetBytes("R"));

                Observable
                .Timer(TimeSpan.FromMilliseconds(1000))
                .Subscribe(
                x =>
                {
                    try
                    {
                        byte[] result = i2cConnection.Read(20);

                        if (result[0] == 1)
                        {
                            String strPH = Encoding.ASCII.GetString(result).Substring(1);
                            Double.TryParse(strPH, out PH);
                            phTile.Set(strPH);
                        }
                        else
                        {
                            phTile.Set("ERR");
                        }
                    }
                    catch (Exception)
                    {
                    }

                    //int[] bytesAsInts = Array.ConvertAll(result, c => (int)c);
                    //phTile.Set(string.Join(" ", bytesAsInts));

                });
            }
            catch (Exception err)
            {
                //phTile.Set(err.Message);
            }
        }
    
        public System.Windows.Forms.Control GetTile()
        {
            return phTile;
        }
    }    
}
