using System;
using System.Windows.Forms;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Pidro.Dialogs
{
    public partial class PinTester : Form
    {
        public PinTester()
        {
            InitializeComponent();
            comboBox1.DataSource = Pi.Gpio.Pins;
            comboBox1.DisplayMember = "HeaderPinNumber";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GpioPin pin = (GpioPin)comboBox1.SelectedItem;
                pin.PinMode = GpioPinDriveMode.Output;
                pin.Write(true);
                
            }
            catch (Exception erf)
            {
                MessageBox.Show(erf.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                GpioPin pin = (GpioPin)comboBox1.SelectedItem;
                pin.PinMode = GpioPinDriveMode.Output;
                pin.Write(false);
            }
            catch (Exception erf)
            {
                MessageBox.Show(erf.Message);
            }
        }
    }
}
