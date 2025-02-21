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

        public FormMathSample()
        {
            InitializeComponent();
        }

        private void FormMathSample_Load(object sender, EventArgs e)
        {
            //Context assignment
            controlNodeEditor.nodesControl.Context = context;
            controlNodeEditor.nodesControl.OnNodeContextSelected += NodesControlOnOnNodeContextSelected; 
            
            //Loading sample from file
            controlNodeEditor.nodesControl.Deserialize(File.ReadAllBytes("default.nds"));
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
                controlNodeEditor.nodesControl.Clear();
                controlNodeEditor.nodesControl.Deserialize(File.ReadAllBytes(ofd.FileName));
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
                byte[] t1 = controlNodeEditor.nodesControl.Serialize();
                File.WriteAllBytes(ofd.FileName, t1);                
            }

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlNodeEditor.nodesControl.Clear();
        }
    }
}
