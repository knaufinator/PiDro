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
    public partial class PressureTile : UserControl
    {
      
        public PressureTile()
        {
            InitializeComponent();
            panel1.BackColor = ComponentBase.colorPallet4;  
            textBox1.BackColor = ComponentBase.colorPallet1;
            textBox1.ForeColor = ComponentBase.colorPallet5;
            button1.BackColor = ComponentBase.colorPallet3;
            label1.ForeColor = ComponentBase.colorPallet3;
        }

        public void set(String pressure)
        {
            textBox1.Text = pressure;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
