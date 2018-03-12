using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pidro.Settings
{

    [Serializable]
    public class W1TempItem : ComponentSetting
    {
        public String Address;
        public String name;
        public Guid ID;

        public W1TempItem()
        {
        }

        public W1TempItem(String Address,String name,Guid ID)
        {
            this.Address = Address;
            this.name = name;
            this.ID = ID;           
        }

        public Guid GetID()
        {
            return ID;
        }

        public override string ToString()
        {
            return name + " W1 Temp";
        }
    }
}
