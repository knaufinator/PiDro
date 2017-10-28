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
    public partial class TemperatureTile : UserControl
    {
        public TemperatureTile()
        {
            InitializeComponent();
            panel1.BackColor = ComponentBase.colorPallet4;  
            textBox1.BackColor = ComponentBase.colorPallet1;
            textBox1.ForeColor = ComponentBase.colorPallet5;
            label1.ForeColor = ComponentBase.colorPallet1;
        }

        public void Set(String temperature)
        {
            textBox1.Text = temperature;
        }
    }
}
