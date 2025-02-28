using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SampleCommon
{
    public partial class PaintsControl : UserControl
    {
        public int rectSize = 2;

        public byte[,] data = new byte[784, 400];
        Bitmap image1;
        public PaintsControl()
        {
            InitializeComponent();
            image1 = new Bitmap(768, 400);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Important - makes sure the Paint event fires
            Draww();
        }
        public void Draww()
        {
            /*using (var pen = Brushes.Red)
            {
                for (int i = 0; i < 384; i++)
                    for (int j = 0; j < 200; j++)
                    {
                    //e.Graphics.DrawRectangle(pen, 0, 0, this.Width, this.Height);
                    g.DrawImage
                    g.FillRectangle(pen, 0 + i * 3, 0+j*3,rectSize,rectSize );
                    }
            }*/


            int x, y;
            Color newColor = Color.FromArgb(0, 0, 0);
            // Loop through the images pixels to reset color.
            for (x = 0; x < image1.Width; x++)
            {
                for (y = 0; y < image1.Height; y++)
                {
                    if ((x % 2 != 0) && (y % 2 != 0))
                    {
                        // Color pixelColor = image1.GetPixel(x, y);

                        if (data[(x - 1)/2, (y - 1)/2] != 0)
                        {
                            image1.SetPixel(x, y, newColor);
                        }
                        else
                            image1.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }

            // Set the PictureBox to display the image.
            pictureBox1.Image = image1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Draww();
        }
    }
}
