using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pidro.Tiles
{
    public partial class TileBase : UserControl
    {
        public TileBase()
        {
            InitializeComponent();
            this.BackColor = ComponentBase.colorPallet4;
            label1.ForeColor = ComponentBase.colorPallet3;
        }
    }
}
