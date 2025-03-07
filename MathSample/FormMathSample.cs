﻿using NodeEditor;
using SampleCommon;
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
    public partial class FormMathSample : Form
    {
        //Context that will be used for our nodes
        MathContext context = new MathContext();
        NodesControlModel model = new NodesControlModel();

        public FormMathSample()
        {
            InitializeComponent();
        }

        private void FormMathSample_Load(object sender, EventArgs e)
        {
            //Context assignment
            model.Context = context;
            controlNodeEditor.nodesControl.OnNodeContextSelected += NodesControlOnOnNodeContextSelected;

            //Loading sample from file
            model.Clear();
            controlNodeEditor.nodesControl.Clear();
          //  model.Deserialize(File.ReadAllBytes("0.nds"));

            controlNodeEditor.nodesControl.model = model;
        }

        private void NodesControlOnOnNodeContextSelected(object o)
        {
            controlNodeEditor.propertyGrid.SelectedObject = o;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "nds";
            ofd.AddExtension = true;
            ofd.Filter = "nds files|*.nds";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                model.Clear();
                controlNodeEditor.nodesControl.Clear();
                model.Deserialize(File.ReadAllBytes(ofd.FileName));
                controlNodeEditor.nodesControl.model = model;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "nds";
            ofd.AddExtension = true;
            ofd.Filter = "nds files|*.nds";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    byte[] t1 = model.Serialize();
                    if ((t1 != null) && (t1.Length > 0))
                    {
                        File.WriteAllBytes(ofd.FileName, t1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.Clear();
            controlNodeEditor.nodesControl.Clear();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.Clear();            
            model.Deserialize(File.ReadAllBytes("0.nds"));
            model.Execute();
            model.Deserialize(File.ReadAllBytes("0.nds"));
            model.Execute();
            controlNodeEditor.nodesControl.Clear();
            controlNodeEditor.nodesControl.model = model;
        }

        private void lOADMEMORYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.DefaultExt = "bin";
                ofd.AddExtension = true;
                ofd.Filter = "bin files|*.bin";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BinaryReader binaryReader = new BinaryReader(File.Open(ofd.FileName, FileMode.Open));
                    if (binaryReader.BaseStream.Length < 65535)
                        return;
                    GlobalData.Instance.globalContext.Memory = new byte[65535];
                    for (int i = 0; i < 65535; i++)
                    {
                        GlobalData.Instance.globalContext.Memory[i] = binaryReader.ReadByte();
                    }
                    binaryReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void sAVEMEMORYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "bin";
            ofd.AddExtension = true;
            ofd.Filter = "bin files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(ofd.FileName));
                    for (int i = 0; i < 65535; i++)
                    {
                        binaryWriter.Write(GlobalData.Instance.globalContext.Memory[i]);
                    }
                    binaryWriter.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
}
