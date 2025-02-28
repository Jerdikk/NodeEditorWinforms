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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DrawSymbol(int x, int y, int symbolCode)
        {
            try
            {
                int yyy = symbolCode << 3;
                for (int i = 0; i < 8; i++)
                {
                    byte b = (byte)data[yyy + i];
                    for (int j = 0; j < 6; j++)
                    {
                        if ((b & (0b1 << j)) == 0)
                        {
                            panels[7 - j, i] = 1;
                        }
                        else
                        {
                            panels[7 - j, i] = 0;
                        }
                    }
                }

                for (int i = 2; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        paintsControl1.data[x * 6 + i, y * 8 + j] = panels[i, j];
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {               
                
                ushort g2 = (ushort)((int.Parse(textBox1.Text)) << 3);

                int counter = 0;

                for (int j = 0; j < 8; j++)
                    for (int i = 0; i < 16; i++)
                    {
                        DrawSymbol(10 + i, 10 + j, counter++);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }
}
