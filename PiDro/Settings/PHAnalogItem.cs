using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pidro.Settings
{
    [Serializable]
    public class PHAnalogItem : ComponentSetting
    {
        public int adcAddress;
        public int phUpPin;
        public int phDownPin;
        public String name;
        public bool autoPHOn;
        public int adcPort;

        public double ph4Voltage;
        public double ph7Voltage;
        public int phIntervalMS;

        public PHAnalogItem()
        {
        }

        public PHAnalogItem(int adcAddress, int adcPort, int phUpPin,int phDownPin,String name,Guid ID,bool autoPHOn,Double ph4,Double ph7, int phIntervalMS)
        {
            this.adcAddress = adcAddress;
            this.phUpPin = phUpPin;
            this.phDownPin = phDownPin;
            this.name = name;
            this.ID = ID;
            this.autoPHOn = autoPHOn;
            this.adcPort = adcPort;

            this.ph4Voltage = ph4;
            this.ph7Voltage = ph7;
            this.phIntervalMS = phIntervalMS;
        }

        public Guid GetID()
        {
            return ID;
        }

        public override string ToString()
        {
            return name + " Analog PH";
        }
    }
}
