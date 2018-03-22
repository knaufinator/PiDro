using MathNet.Numerics;
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Linq;
using RaspberrySharp.IO.InterIntegratedCircuit;
using RaspberrySharp.IO.GeneralPurpose;
using Pidro.Tools;
using Pidro.Settings;
using System.Windows.Forms;

namespace Pidro.Tiles
{
    public class PHComponent : ComponentInterface
    {
        private PHTile phTile = new PHTile();

        private int phdownHeaderPinNumber = 0;
        private int phupHeaderPinNumber = 0;
        private List<double> phHistory = new List<double>();

        private int mode;//1 - ezo, 2 analog

        private Guid ID;
        private IDisposable subscription;
        private AppSettings settings = AppSettings.Instance;
        private Boolean autoOn = false;

        private int pumpOnTimeMilliSeconds = 1000;
        private int pumpAutoOnTimeMilliSeconds = 500;
         
        private I2cDriver driver;//ezo
        private I2cDeviceConnection i2cConnection;
        
        ADConverter aDConverter;
        private int sensorId;
        private double targetPH = 5.8;
        private double phTolerance = .15;
        
        public PHComponent() 
        {
            phTile.button1.Click += Cal_Click;
            phTile.button2.Click += Auto_Click;
            phTile.button3.Click += Up_Click;
            phTile.button4.Click += Down_Click;
            phTile.label2.Text = targetPH.ToString();

            //update the ph value, every 2 seconds,
            IObservable<long> timer = Observable.Timer(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(2000));
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

        public PHComponent(Settings.PHEZOItem item): this()
        {
            mode = 1;
            ID = item.ID;

            phdownHeaderPinNumber = item.phDownPin;
            phupHeaderPinNumber = item.phUpPin;

            try
            {
                GpioPin p1 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phupHeaderPinNumber);
                if (p1 != null)
                    p1.PinMode = GpioPinDriveMode.Output;

                GpioPin p2 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phdownHeaderPinNumber);
                if (p2 != null)
                    p2.PinMode = GpioPinDriveMode.Output;
            }
            catch
            {
            }


            if (item.autoPHOn)
                EnableAutoOn();
            else
                DisableAutoOn();

            try
            {
                driver = new I2cDriver(ProcessorPin.Gpio02, ProcessorPin.Gpio03);//standard ports on raspi
                i2cConnection = driver.Connect(item.I2cAddress);
            }
            catch
            {
            }
        }

        public PHComponent(Settings.PHAnalogItem item) : this()
        {
            mode = 2;

            phdownHeaderPinNumber = item.phDownPin;
            phupHeaderPinNumber = item.phUpPin;
            sensorId = item.adcPort;
            ID = item.ID;

            try
            {
                GpioPin p1 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phupHeaderPinNumber);
                if (p1 != null)
                    p1.PinMode = GpioPinDriveMode.Output;

                GpioPin p2 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == phdownHeaderPinNumber);
                if (p2 != null)
                    p2.PinMode = GpioPinDriveMode.Output;
            }
            catch
            {
            }


            if (item.autoPHOn)
                EnableAutoOn();
            else
                DisableAutoOn();
            try
            {
                aDConverter = new ADConverter(item.adcAddress);
            }
            catch (Exception e)
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

        private void Cal_Click(object sender, EventArgs e)
        {
            CalibratePHAuto();
        }

        public void CalibratePHAuto()
        {
            PHAnalogItem setting = (PHAnalogItem) settings.CurrentSettings.GetComponent(ID);

            if (setting != null)
            {
                Double phV = aDConverter.GetADVoltage(sensorId);
                Double vCutoff = 1.5;

                if (phV <= vCutoff)
                {
                    if (MessageBox.Show("4 PH ? @ " + phV.ToString() + "V", "Calibrate 4 PH", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        setting.ph4Voltage = phV;
                        settings.Save();
                    }
                }
                else
                {
                    if (MessageBox.Show("7 PH ? @ " + phV.ToString() + "V", "Calibrate 70 PH", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        setting.ph7Voltage = phV;
                        settings.Save();
                    }
                }
                

                settings.Save();
            }
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

        public Double getAnalogPH()
        {
            double result = 0.0;

            PHAnalogItem setting = (PHAnalogItem)settings.CurrentSettings.GetComponent(ID);

            if (setting.ph4Voltage > 0 & setting.ph7Voltage > 0)
            {
                double[] x = { setting.ph4Voltage, setting.ph7Voltage };
                double[] y = { 4, 7 };

                try
                {
                    Tuple<double, double> p = Fit.Line(x, y);
                    double c = p.Item1;
                    double m = p.Item2;

                    //y = mx + c;
                    result = m * aDConverter.GetADVoltage(sensorId) + c;    
                }
                catch (Exception e)
                {
                }
            }

            return result;
        }

        private void TryPinWrite(GpioPin pin, bool level)
        {
            try
            {
                pin.PinMode = GpioPinDriveMode.Output;
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
            if (mode == 1)
                UpdateEZO();
            else
                UpdateAnalog();  
        }

        private void UpdateAnalog()
        {
            UpdatePH(getAnalogPH());
        }

        private void UpdateEZO()
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
                            UpdatePH(phResult);
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

        private void UpdatePH(double phResult)
        {
            phHistory.Add(phResult);

            //keep 10 in history
            if (phHistory.Count > 10)
            {
                phHistory.RemoveAt(0);
            }

            phTile.Set(String.Format("{0:F2}", phResult));
        }

        public System.Windows.Forms.Control GetTile()
        {
            return phTile;
        }
    }    
}
