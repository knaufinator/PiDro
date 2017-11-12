using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Pidro.Tiles
{
    public partial class PHControlTile : Pidro.Tiles.TileBase
    {
        public PHControlTile()
        {
            InitializeComponent();
            phValue.BackColor = ComponentBase.colorPallet1;
            phValue.ForeColor = ComponentBase.colorPallet5;
            titleLabel.Text = "PH";

            button1.BackColor = ComponentBase.colorPallet3;
            button2.BackColor = ComponentBase.colorPallet3;
            button3.BackColor = ComponentBase.colorPallet3;
            button4.BackColor = ComponentBase.colorPallet3;
        }
    }
}
