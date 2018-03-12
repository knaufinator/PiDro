using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pidro.Settings
{
    [Serializable]
    public class PHEZOItem : ComponentSetting
    {
        public int I2cAddress;
        public int phUpPin;
        public int phDownPin;
        public String name;
        public bool autoPHOn;


        public PHEZOItem()
        {
           
        }

        public PHEZOItem(int I2cAddress,int phUpPin,int phDownPin,String name,Guid ID,bool autoPHOn)
        {
            this.I2cAddress = I2cAddress;
            this.phUpPin = phUpPin;
            this.phDownPin = phDownPin;
            this.name = name;
            this.ID = ID;
            this.autoPHOn = autoPHOn;
        }

        public Guid GetID()
        {
            return ID;
        }

        public override string ToString()
        {
            return name + " EZO PH";
        }
    }
}
