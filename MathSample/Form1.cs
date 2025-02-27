using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MathSample
{
    public partial class Form1 : Form
    {
        Panel[,] panels = new Panel[8, 8];
        byte[] data;
        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    panels[i, j] = new Panel();
                    tableLayoutPanel1.Controls.Add(panels[i, j], i, j);
                }
            try
            {
                BinaryReader binaryReader = new BinaryReader(File.Open("zg.rom", FileMode.Open));

                data = new byte[binaryReader.BaseStream.Length];
                for (int k = 0; k < binaryReader.BaseStream.Length; k++)
                {
                    data[k] = binaryReader.ReadByte();
                }

                binaryReader.Close();
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string t1 = textBox1.Text;
            int g1 = 0;

            ushort g2 = 0;

            try
            {
                g1 = int.Parse(t1);
                g2 = (ushort)(g1 << 3);
            }
            catch { }

           // byte[] bb = new byte[64];
          //  int counter = 0;

            for (int i = 0; i < 8; i++)
            {
                byte b = (byte)data[g2 + i];
                for (int j = 0; j < 6; j++)
                {
                    if ((b & (0b1 << j)) == 0)
                    {
                  //      bb[counter] = 1;
                        panels[7-j, i].BackColor = Color.Black;
                    }
                    else
                    {
                   //     bb[counter] = 0;
                        panels[7-j, i].BackColor = Color.LightGray;
                    }
                   // counter++;
                }
                             
            }
            /*
            counter = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 6; j++)
                {
                    if (bb[counter] == 1)
                        panels[i, j].BackColor = Color.Black;
                    else
                        panels[i, j].BackColor = Color.White;
                    counter++;
                }*/

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
