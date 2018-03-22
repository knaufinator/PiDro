using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pidro.Settings
{
    [Serializable]
    public class IntervalRelayItem : ComponentSetting
    {
        public int relayPin;
        public String name;

        public int timeOnSec;
        public int timeOffSec;

        public IntervalRelayItem()
        {
        }

        public IntervalRelayItem(int relayPin,String name,int timeOnSec,int timeOffSec,Guid ID)
        {
            this.relayPin = relayPin;
            this.name = name;
            this.ID = ID;
            this.timeOnSec = timeOnSec;
            this.timeOffSec = timeOffSec;
        }

        public Guid GetID()
        {
            return ID;
        }

        public override string ToString()
        {
            return name + " Interval Relay";
        }
    }
}
