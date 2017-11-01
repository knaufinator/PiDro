using Pidro.Dialogs;
using Pidro.Tiles;
using Mono.Nat;
using System;
using System.Linq;
using System.Windows.Forms;
using Pidro.Tools;
using System.Threading.Tasks;
using WampSharp.V2;
using Pidro.Service;
using WampSharp.V2.Client;
using SystemEx;

namespace Pidro
{
    public partial class MainWindow : Form
    {
       // AppSettings settings = AppSettings.Instance;

        //this will be in xml config later...
        //ports that will be opened up, internal for when we have multiple pis, and want to just use dif port mappings
        int[] externalPorts = new int[] { 8123, 5900, 22 };
        //int[] internalPorts = new int[] { 8123, 5900, 22 };
        
        public MainWindow()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            //setupUPNP();//uncomment to make upnp forward your ports
           // setupWampAsync();//testing 


            ADConverter aDConverter = new ADConverter(0x48);
            
            //create dynamic builder, from config xml, generate each component
            TimerComponent timerComponent = new TimerComponent(300, 5);
            PHComponent pHComponent = new PHComponent(aDConverter);
            PressureComponent pressureComponent = new PressureComponent(aDConverter);
            TemperatureComponent temperatureComponent = new TemperatureComponent();

            //pin tester, in order to hunt down what pins have relays attached
            UserControl1 test = new UserControl1();

            this.flowLayoutPanel1.Controls.Add(timerComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(pHComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(pressureComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(temperatureComponent.GetTile());
            this.flowLayoutPanel1.Controls.Add(test);
        }

        private async void setupWampAsync()
        {
            const string location = "ws://127.0.0.1:8080/";

            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            IWampChannel channel = channelFactory.CreateJsonChannel(location, "realm1");

            await channel.Open().ConfigureAwait(false);

            IWebService instance = new WebService();

            IWampRealmProxy realm = channel.RealmProxy;

            IAsyncDisposable disposable = await realm.Services.RegisterCallee(instance).ConfigureAwait(false);
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

                //remove existing port maps that exist on the ports we want
                foreach (Mapping portMap in device.GetAllMappings())
                    if (externalPorts.Contains(portMap.PublicPort))
                    {
                        Console.WriteLine("Deleting " + portMap.PublicPort.ToString());
                        device.DeletePortMap(portMap);
                    }


                //add the ports we want for our IP
                foreach (int port in externalPorts)
                {   
                    Console.WriteLine("Creating IP: " + device.LocalAddress.ToString()+ " port: " + port.ToString());
                    device.CreatePortMap(new Mapping(Protocol.Tcp, port, port));
                }
                    
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        private void arcTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArcTest arcTest = new ArcTest();
            arcTest.Show();
        }
    }    
}
