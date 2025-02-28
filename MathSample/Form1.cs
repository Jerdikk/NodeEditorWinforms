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
        byte[,] panels = new byte[8, 8];
        byte[] data;
        public Form1()
        {
            InitializeComponent();

            /*for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    panels[i, j] = new Panel();
                    tableLayoutPanel1.Controls.Add(panels[i, j], i, j);
                }*/
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

        public void DrawSymbol(int x, int y, int symbolCode)
        {
            int yyy = symbolCode << 3;
            for (int i = 0; i < 8; i++)
            {
                byte b = (byte)data[yyy + i];
                for (int j = 0; j < 6; j++)
                {
                    if ((b & (0b1 << j)) == 0)
                    {
                        //      bb[counter] = 1;
                        panels[7 - j, i] = 1;//.BackColor = Color.Black;
                    }
                    else
                    {
                        //     bb[counter] = 0;
                        panels[7 - j, i] = 0;//.BackColor = Color.LightGray;
                    }
                    // counter++;
                }

            }

            for (int i = 2; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    paintsControl1.data[x * 6 + i, y * 8 + j] = panels[i, j];
                }

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

            int counter = 0;

            for (int j = 0; j < 8; j++)
                for (int i = 0; i < 16; i++)
                {
                    DrawSymbol(10 + i, 10 + j, counter++);
                }         

        }

    }
}
