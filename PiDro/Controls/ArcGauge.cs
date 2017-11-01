using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Pidro.Controls
{
    public partial class ArcGauge : UserControl
    {
        private int _width;
        private int _height;
        private int _arcBrushSize = 50;

        int _w = 48;
        int _outerWidth = 16;
        Color _color1 = Color.Red;
        Color _color2 = Color.Blue;
        float _sAngle = 180;
        float _swAngle = 180;
        Rectangle _r;


        public ArcGauge(int width, int height)
        {
            _width = width;
            _height = height;
            InitializeComponent();
            this.Size = new Size(width, height);

            _r = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);

        }

        private void ArcGauge_Paint(object sender, PaintEventArgs e)
        {


            using (System.Drawing.Drawing2D.GraphicsPath gPath = new System.Drawing.Drawing2D.GraphicsPath())
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.High;

                //antialias the outer edge
                using (Pen pen = new Pen(_color1, 2))
                    e.Graphics.DrawArc(pen, _r, _sAngle, _swAngle);

                gPath.AddArc(_r, _sAngle, _swAngle);
                using (System.Drawing.Drawing2D.PathGradientBrush p = new System.Drawing.Drawing2D.PathGradientBrush(gPath))
                {
                    p.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

                    System.Drawing.Drawing2D.Blend b = new System.Drawing.Drawing2D.Blend();
                    b.Factors = new float[] { 1, 1, 0, 0, 0 };
                    b.Positions = CalcPositions(_r, _outerWidth);
                    p.Blend = b;

                    p.CenterColor = _color1;
                    p.CenterPoint = new PointF(_r.Width / 2f + _r.X, _r.Height / 2f + _r.Y);
                    //p.SurroundColors = new Color[] { _color2 };

                    using (Pen pen = new Pen(p, _w))
                        e.Graphics.DrawArc(pen, _r, _sAngle, _swAngle);
                }




                // e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                // e.Graphics.InterpolationMode = InterpolationMode.High;

                // Pen redPen = new Pen(Color.Red, _arcBrushSize);
                // redPen.SetLineCap(LineCap.Flat, LineCap.Flat, DashCap.Flat);

                // Rectangle rect = new Rectangle(_arcBrushSize/2, _arcBrushSize/2, _width - _arcBrushSize, _height);
                // e.Graphics.DrawArc(redPen, rect, 180, 180);

                //// Pen bluePen = new Pen(Color.Blue, _arcBrushSize);
                //// e.Graphics.DrawArc(bluePen, rect, 180, 50);


                // int textx = _width / 2;
                // int texty = _height / 2;

                // String text = "5.7";
                // SizeF s = e.Graphics.MeasureString(text, new Font("Arial", 25, FontStyle.Bold));
                // e.Graphics.DrawString("5.7", new Font("Arial", 25, FontStyle.Bold), Brushes.Black, textx- s.Width/2, texty - s.Height/2);

                // bluePen.Dispose();
                // redPen.Dispose();
            }

        }
        
        private float[] CalcPositions(Rectangle rectangle, double w)
        {
            float[] f = new float[5];
            f[0] = 0;
            f[4] = 1;
            double x = w / (double)rectangle.Width;
            f[1] = (float)(x - 0.001);
            f[2] = (float)x;
            f[3] = (float)(x + 0.001);

            return f;
        }
    }
}
