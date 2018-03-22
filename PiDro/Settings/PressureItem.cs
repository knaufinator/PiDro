using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pidro.Settings
{
    [Serializable]
    public class PressureItem : ComponentSetting
    {
        public int adcAddress;
        public int adcPort;

        public String name;

        internal double pressure110;
        internal double pressure80;

        public PressureItem()
        {
        }

        public PressureItem(int adcAddress, string name,double pressure110, double pressure80, Guid ID)
        {
            this.adcAddress = adcAddress;
            this.name = name;
            this.ID = ID;
            this.pressure80 = pressure80;
            this.pressure110 = pressure110;
        }

        public Guid GetID()
        {
            return ID;
        }

        public override string ToString()
        {
            return name + " Pressure";
        }
    }
}
