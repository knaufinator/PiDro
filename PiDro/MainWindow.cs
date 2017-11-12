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
using WampSharp.V2.Realm;
using System.Reactive.Subjects;
using System.Reactive.Linq;

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
            //  this.WindowState = FormWindowState.Maximized;
            Size = new System.Drawing.Size(800, 480);
            //setupUPNP();//uncomment to make upnp forward your ports
            SetupWamp();//testing 


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

        private void SetupWamp()
        {          
            try
            {
                string location = "ws://" + NetworkTool.GetLocalIPAddress() +":8080/ws";
                DefaultWampHost host = new DefaultWampHost(location);
                host.Open();
                
                IWampHostedRealm realm = host.RealmContainer.GetRealmByName("data");

                ISubject<int> subject =
                    realm.Services.GetSubject<int>("com.pidra.data");

                int counter = 0;
                IObservable<long> timer = Observable.Timer(TimeSpan.FromMilliseconds(0),TimeSpan.FromMilliseconds(1000));

                IDisposable disposable =
                    timer.Subscribe(x =>
                    {
                        counter++;
                        try
                        {
                            subject.OnNext(counter);
                        }
                        catch (Exception ex)
                        {
                        }
                    });                
            }
            catch (Exception e)
            {
                String test = "";
            }
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
