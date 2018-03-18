using Pidro.Settings;
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

        List<ComponentSetting> settingsList = new List<ComponentSetting>();
        
        public SettingsForm()
        {
            InitializeComponent();
            settings = AppSettings.Instance;
            LoadSettings();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadSettings()
        {
            //clear list
            settingsList.Clear();

            AppSettings.Instance.LoadSettings();
          
            //get settings
            settingsList = AppSettings.Instance.CurrentSettings.Components;

            listBox1.DataSource = null;
            listBox1.DataSource = settingsList;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //take params ask user if OK
            //add setting if ok
            String i2cAddressStrInt = textBox1.Text;
            String phUpPinStrInt = textBox2.Text;
            String phDownPinStrInt = textBox3.Text;
            String name = textBox11.Text;

            int i2cAddressInt = 0;
            int phUpPinInt = 0;
            int phDownPinInt = 0;            

            int.TryParse(i2cAddressStrInt, out i2cAddressInt);
            int.TryParse(phUpPinStrInt, out phUpPinInt);
            int.TryParse(phDownPinStrInt, out phDownPinInt);
            
            Guid id = Guid.NewGuid();
            String idStr = id.ToString();

            PHEZOItem item = new PHEZOItem(i2cAddressInt, phUpPinInt, phDownPinInt, name, id, checkBox1.Checked);
            settings.SaveComponent(item);
            LoadSettings();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //remove selected setting
            var item = listBox1.SelectedItem;
            Guid id = ((ComponentSetting)item).ID;

            //settings.DeleteSetting(id);
            
            LoadSettings();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //add w1 temp component
            String name = textBox8.Text;
            String address = textBox6.Text;

            Guid id = Guid.NewGuid();
            String idStr = id.ToString();

            W1TempItem item = new W1TempItem(address, name, id);
            settings.SaveComponent(item);

            LoadSettings();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //take params ask user if OK
            //add setting if ok
            String adcI2cAddressStr = textBox12.Text;
            String adcPortStr = textBox15.Text;
            String phUpPinStrInt = textBox14.Text;
            String phDownPinStrInt = textBox13.Text;
            String name = textBox10.Text;

            String ph4VStr = textBox5.Text;
            String ph7VStr = textBox4.Text;

            int adcI2cAddressInt = 0;
            int phUpPinInt = 0;
            int phDownPinInt = 0;
            int adcPortInt = 0;
            Double ph4V = 0;
            Double ph7V = 0;

            int.TryParse(adcI2cAddressStr, out adcI2cAddressInt);
            int.TryParse(phUpPinStrInt, out phUpPinInt);
            int.TryParse(phDownPinStrInt, out phDownPinInt);
            int.TryParse(adcPortStr, out adcPortInt);

            Double.TryParse(ph4VStr, out ph4V);
            Double.TryParse(ph7VStr, out ph7V);

            Guid id = Guid.NewGuid();
            String idStr = id.ToString();

            PHAnalogItem item = new PHAnalogItem(adcI2cAddressInt,adcPortInt, phUpPinInt, phDownPinInt, name, id, checkBox1.Checked, ph4V, ph7V);
            settings.SaveComponent(item);
            LoadSettings();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //cear settings
            settings.ClearSettings();
            LoadSettings();
        }
    }
}
