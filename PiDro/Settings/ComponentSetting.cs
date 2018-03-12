using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pidro.Settings
{
    [Serializable]
    public class ComponentSetting
    {
        private Guid iD;
        public Guid ID { get => iD; set => iD = value; }
    }
}
