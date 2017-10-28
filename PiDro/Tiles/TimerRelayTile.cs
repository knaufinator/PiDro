using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pidro.Tiles
{
    public partial class TimerRelayTile : UserControl
    {
        Image triggeredOnIcon;
        Image triggeredOffIcon;

        public TimerRelayTile(String name)
        {
            InitializeComponent();
            label1.Text = name;
            label1.ForeColor = ComponentBase.colorPallet1;
            triggeredOnIcon = new Bitmap(Properties.Resources.on_icon);
            triggeredOffIcon = new Bitmap(Properties.Resources.off_icon);
            panel1.BackColor = ComponentBase.colorPallet4;  
            textBox1.BackColor = ComponentBase.colorPallet1;
            textBox1.ForeColor = ComponentBase.colorPallet5;            
        }

        public void set(String time,Boolean triggered)
        {
            textBox1.Text = time;
            if (triggered)
                pictureBox1.Image = triggeredOnIcon;
            else
                pictureBox1.Image = triggeredOffIcon;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            //edit here
        }
    }
}
