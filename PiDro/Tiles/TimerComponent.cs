using Pidro.Settings;
using Pidro.Tiles;
using System;
using System.Linq;
using System.Windows.Forms;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Pidro.Tiles
{
    public class TimerComponent :ComponentInterface
    {
        TimerRelayTile trPanel;

        Boolean triggerOn = false;
        DateTime TriggerOnTime;
        DateTime TriggerOffTime;

        Timer updateTimer;
        Timer triggerOnTimer;
        Timer triggerOffTimer;

        private TimeSpan triggerDelaySecSpan;
        private TimeSpan triggerOnTimeSecSpan;

        private int triggerDelaySec;
        private int triggerOnTimeSec;

        private int relayPort;
        private TimerRelayTile item;

        public TimerComponent(IntervalRelayItem intervalRelayItem)
        {
            trPanel = new TimerRelayTile(intervalRelayItem.name);
            relayPort = intervalRelayItem.relayPin;

            this.triggerDelaySec = intervalRelayItem.timeOffSec;
            this.triggerOnTimeSec = intervalRelayItem.timeOnSec;

            this.triggerDelaySecSpan = new TimeSpan(0, 0, triggerDelaySec);
            this.triggerOnTimeSecSpan = new TimeSpan(0, 0, triggerOnTimeSec);

            //update the clock readout one a sec
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;

            SetTime();

            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            String result = "";
            TimeSpan timeleft;
            if (triggerOn)
                timeleft = triggerOnTimeSecSpan - (DateTime.Now - TriggerOnTime);
            else
                timeleft = triggerDelaySecSpan - (DateTime.Now - TriggerOffTime);
            
            result = timeleft.Hours.ToString() + "h" + timeleft.Minutes.ToString() + "m " + timeleft.Seconds.ToString() + "s";
            trPanel.set(result, triggerOn);           
        }

        public void SetTime()
        {
            if (triggerOnTimer != null)
                triggerOnTimer.Stop();           

            if (triggerOffTimer != null)
                triggerOffTimer.Stop();

            triggerOnTimer = new Timer();
            triggerOnTimer.Interval = triggerDelaySec * 1000;
            triggerOnTimer.Tick += triggerOnTimer_Tick;
        
            triggerOffTimer = new Timer();
            triggerOffTimer.Interval = triggerOnTimeSec * 1000;
            triggerOffTimer.Tick += triggerOffTimer_Tick;

            //start the delay till trigger
            TriggerOffTime = DateTime.Now;
            triggerOnTimer.Start();
        }

        private void triggerOnTimer_Tick(object sender, EventArgs e)
        {
            triggerOnTimer.Stop();

            try
            {
                GpioPin p1 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == relayPort);
                p1.PinMode = GpioPinDriveMode.Output;
                p1.Write(true);
            }
            catch (Exception err)
            {
                
            }

            TriggerOnTime = DateTime.Now;
            triggerOn = true;

            //open pin
            triggerOffTimer.Start();   
        }

        private void triggerOffTimer_Tick(object sender, EventArgs e)
        {
            triggerOffTimer.Stop();

            try
            {
                GpioPin p1 = Pi.Gpio.Pins.FirstOrDefault(i => i.HeaderPinNumber == relayPort);
                p1.Write(false);
            }
            catch (Exception)
            {
                String test = "";
            }
            TriggerOffTime = DateTime.Now;
            triggerOn = false;

            triggerOnTimer.Start();
        }

        private void stopTimer()
        {
            
        }

        public Control GetTile()
        {
            return trPanel;
        }

    }
}
