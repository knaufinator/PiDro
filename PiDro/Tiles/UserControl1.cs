using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Pidro.Tiles
{
    public partial class UserControl1 : Pidro.Tiles.TileBase
    {
        public UserControl1()
        {
            InitializeComponent();
            
            try
            {
                comboBox1.DataSource = Pi.Gpio.Pins;
                comboBox1.DisplayMember = "HeaderPinNumber";
            }
            catch (Exception e)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //get selected pin
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
            //get selected pin
            try
            {
                GpioPin pin = (GpioPin)comboBox1.SelectedItem;
                pin.PinMode = GpioPinDriveMode.Output;
                pin.Write(false);
            }
            catch (Exception re)
            {
                MessageBox.Show(re.Message);
            }
        }
    }
}
