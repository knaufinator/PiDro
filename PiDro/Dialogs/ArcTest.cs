using Pidro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pidro.Dialogs
{
    public partial class ArcTest : Form
    {
        public ArcTest()
        {
            InitializeComponent();

            ArcGauge d = new ArcGauge(180,180);


            this.Controls.Add(d);
        }

 
    }
}
