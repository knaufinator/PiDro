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
            
            //get settings
           // settingsList = settings.GetSettings();
           // listBox1.DataSource = settingsList;
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

            //add setting for this 

            PHEZOItem item = new PHEZOItem(i2cAddressInt, phUpPinInt, phDownPinInt, name, id, checkBox1.Checked);
            settings.SaveComponent(item);
            //settings.SaveSetting(idStr, "Name", name);
            //settings.SaveSetting(idStr, "Type", "PHEZO");
            //settings.SaveSetting(idStr, "PHI2cAddress", i2cAddressInt.ToString());
            //settings.SaveSetting(idStr, "PHUpPin", phUpPinInt.ToString());
            //settings.SaveSetting(idStr, "PHDownPin", phDownPinInt.ToString());
            //settings.SaveSetting(idStr, "AutoOn", checkBox1.Checked.ToString());

            LoadSettings();
        }

        private void button12_Click(object sender, EventArgs e)
        {

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
            String address = textBox8.Text;

            Guid id = Guid.NewGuid();
            String idStr = id.ToString();

            W1TempItem item = new W1TempItem(address, name, id);
            settings.SaveComponent(item);

            LoadSettings();
        }
    }
}
