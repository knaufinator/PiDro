using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pidro.Dialogs
{
    public partial class SettingsForm : Form
    {

        AppSettings settings;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            try
            {
                settings = AppSettings.Instance;
                MessageBox.Show("done");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                String test =settings.GetSetting("PH", "PH4", "1");

                MessageBox.Show(test);

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                settings.SaveSetting("PH", "PH4", "3");

                MessageBox.Show("done");

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
    }
}
