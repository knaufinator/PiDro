using HydroTest.Tiles;
using Mono.Nat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HydroTest
{
    public partial class MainWindow : Form
    {
        AppSettings settings = AppSettings.Instance;

        //this will be in xml config later...
        //ports that will be opened up, internal for when we have multiple pis, and want to just use dif port mappings
        int[] externalPorts = new int[] { 8123, 5901, 5800, 22 };
        //int[] internalPorts = new int[] { 8123, 5901, 5800, 22 };
        
        public MainWindow()
        {
            InitializeComponent();
            this.Size = new Size(800, 480);
            //this.max
           // setupUPNP();//uncomment to make upnp grab forward your ports


            //create dynamic builder, from config xml, generate each component
            TimerComponent timerComponent = new TimerComponent(300, 5);
            PHComponent pHComponent = new PHComponent();
            PressureComponent pressureComponent = new PressureComponent();
            TemperatureComponent temperatureComponent = new TemperatureComponent();

            this.WindowState = FormWindowState.Maximized; 
            this.flowLayoutPanel1.Controls.Add(timerComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(pHComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(pressureComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(temperatureComponent.GetTile());
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void setupUPNP()
        {
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.DeviceLost += DeviceLost;
            NatUtility.StartDiscovery();
        }

        private void DeviceFound(object sender, DeviceEventArgs args)
        {
            try
            {
                INatDevice device = args.Device;

                foreach (Mapping portMap in device.GetAllMappings())
                    if(externalPorts.Contains(portMap.PublicPort))
                        device.DeletePortMap(portMap);
                
                foreach (int port in externalPorts)
                    device.CreatePortMap(new Mapping(Protocol.Tcp, port, port));
            }
            catch (Exception e)
            {
              
            }
        }
        
        private void DeviceLost(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }    
}
