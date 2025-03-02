using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SampleCommon
{
    public partial class ControlNodeEditor : UserControl
    {
        public ControlNodeEditor()
        {
            InitializeComponent();
            accTextBox.Text = GlobalData.Instance.globalContext.A.ToString();
            cfTextBox.Text = GlobalData.Instance.globalContext.Flags.ToString();
            prcTextBox.Text = GlobalData.Instance.globalContext.ProgrammCounter.ToString();
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            nodesControl.model.Execute();
            accTextBox.Text = GlobalData.Instance.globalContext.A.ToString();
            cfTextBox.Text = GlobalData.Instance.globalContext.Flags.ToString();
            prcTextBox.Text = GlobalData.Instance.globalContext.ProgrammCounter.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string t1 = textBox1.Text;

                if (t1.Length > 0)
                {
                    int g = int.Parse(t1);
                    g &= 65535;

                    textBox2.Text = GlobalData.Instance.globalContext.Memory[g].ToString();

                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string t1 = textBox1.Text;

                if (t1.Length > 0)
                {
                    string t2 = textBox2.Text;
                    if (t2.Length > 0)
                    {
                        int g = int.Parse(t1);
                        g &= 65535;
                        int g1 = int.Parse(t2);
                        g1 &= 255;
                        GlobalData.Instance.globalContext.Memory[g]=(byte)g1;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
