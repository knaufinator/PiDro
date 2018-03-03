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
    public partial class PHTile : UserControl
    {
      
        public PHTile()
        {
            InitializeComponent();
            panel1.BackColor = ComponentBase.colorPallet4;  
            textBox1.BackColor = ComponentBase.colorPallet1;
            textBox1.ForeColor = ComponentBase.colorPallet5;
            button1.BackColor = ComponentBase.colorPallet3;
            label1.ForeColor = ComponentBase.colorPallet3;
        }

        public void Set(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(Set), new object[] { value });
                return;
            }
            textBox1.Text = value;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
