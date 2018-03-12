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
using System.Linq;
using RaspberrySharp.IO.InterIntegratedCircuit;
using RaspberrySharp.IO.GeneralPurpose;

namespace Pidro.Tiles
{
    public class PHComponent : ComponentInterface
    {
        private PHTile phTile = new PHTile();

        private int phdownHeaderPinNumber = 0;
        private int phupHeaderPinNumber = 0;
        private List<double> phHistory = new List<double>();

        private IDisposable subscription;
        private AppSettings settings = AppSettings.Instance;
        private Boolean autoOn = false;

        private int pumpOnTimeMilliSeconds = 1000;
        private int pumpAutoOnTimeMilliSeconds = 500;
         
        private static I2cDriver driver;
        private static I2cDeviceConnection i2cConnection;

        private double targetPH = 5.8;
        private double phTolerance = .1;
        private double PH = 0;

        public PHComponent(Settings.PHEZOItem item)
        { 
            //phTile.button1.Click += Cal_Click;
            phTile.button2.Click += Auto_Click;
            phTile.button3.Click += Up_Click;
            phTile.button4.Click += Down_Click;
            phTile.label2.Text = targetPH.ToString();

            //update the ph value, every 2 seconds,
            IObservable<long> timer = Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(2000));
            IDisposable disposable =
                timer.Subscribe(x =>{
                    try
                    {
                        Update();
                    }
                    catch (Exception ex)
                    {
                    }
                });

            phdownHeaderPinNumber = item.phDownPin;
            phupHeaderPinNumber = item.phUpPin;

            if (item.autoPHOn)
                EnableAutoOn();
            else
                DisableAutoOn();

            try
            {
                GpioPin p1 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phupHeaderPinNumber);
                if (p1 != null)
                    p1.PinMode = GpioPinDriveMode.Output;

                GpioPin p2 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phdownHeaderPinNumber);
                if (p2 != null)
                    p2.PinMode = GpioPinDriveMode.Output;

                //if we ahve multiple of these does this cuase a problem?,... may need to create a global version and pass it to i2c devices
                driver = new I2cDriver(ProcessorPin.Gpio02, ProcessorPin.Gpio03);//standard ports on raspi
                i2cConnection = driver.Connect(item.I2cAddress);
            }
            catch
            {
            }
        }

        private void Auto_Click(object sender, EventArgs e)
        {
            if (autoOn == false)
                EnableAutoOn();
            else
                DisableAutoOn();
        }

        private void EnableAutoOn()
        {
            phTile.button2.Text = "On";
            autoOn = true;

            //sample every x seconds, if over target, apply correct dose up/down, once 
            subscription = Observable
                .Interval(TimeSpan.FromSeconds(300))//5 min
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

        private void DisableAutoOn()
        {
            phTile.button2.Text = "OFF";
            autoOn = false;

            if (subscription != null)
                subscription.Dispose();
        }

        BehaviorSubject<bool> switches = new BehaviorSubject<bool>(false);
         
        private void CheckPHAndDose()
        {
            double phAvg = phHistory.Average();

            if (phAvg > targetPH + phTolerance)
            {
                //dose ph down,
                PHDownDose(pumpAutoOnTimeMilliSeconds);
            }
            else if (phAvg < targetPH - phTolerance)
            {
                PHUpDose(pumpAutoOnTimeMilliSeconds);
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
            PHUpDose(pumpOnTimeMilliSeconds);
        }
        
        private void Down_Click(object sender, EventArgs e)
        {
            PHDownDose(pumpOnTimeMilliSeconds);
        }

        private void PHDownDose(int milliseconds)
        {
            RunPump(Pi.Gpio.Pins.FirstOrDefault(i=>i.HeaderPinNumber == phdownHeaderPinNumber), milliseconds);
        }

        private void PHUpDose(int milliseconds)
        {
            RunPump(Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phupHeaderPinNumber), milliseconds);
        }

        private void RunPump(GpioPin pin, int onTimeMilliseconds)
        {
            if (pin == null)
                return;

            TryPinWrite(pin, true);

            Observable
              .Timer(TimeSpan.FromMilliseconds(onTimeMilliseconds))
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

                //ezo needs 900 milliseconds to get a value ready
                Observable
                .Timer(TimeSpan.FromMilliseconds(900))
                .Subscribe(
                x =>
                {
                    try
                    {
                        byte[] result = i2cConnection.Read(20);

                        if (result[0] == 1)
                        {
                            String strPH = Encoding.ASCII.GetString(result).Substring(1);
                            Double phResult = 0;

                            Double.TryParse(strPH, out phResult);
                            phHistory.Add(phResult);

                            //keep 10 in history
                            if (phHistory.Count > 10)
                            {
                                phHistory.RemoveAt(0);
                            }

                            //global value for when the ui wants to update.
                            PH = phResult;
                            phTile.Set(String.Format("{0:F1}", phResult));
                        }
                        else
                        {
                            phTile.Set("ERR");
                        }
                    }
                    catch (Exception)
                    {
                    }
                });
            }
            catch (Exception err)
            {
            }
        }
    
        public System.Windows.Forms.Control GetTile()
        {
            return phTile;
        }
    }    
}
