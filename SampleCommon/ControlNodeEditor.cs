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
            accTextBox.Text = GlobalData.Instance.globalContext.Accumulator.ToString();
            cfTextBox.Text = GlobalData.Instance.globalContext.CarryFlag.ToString();
            prcTextBox.Text = GlobalData.Instance.globalContext.ProgrammCounter.ToString();
        }

        private void buttonProcess_Click(object sender, EventArgs e)
        {
            nodesControl.model.Execute();
            accTextBox.Text = GlobalData.Instance.globalContext.Accumulator.ToString();
            cfTextBox.Text = GlobalData.Instance.globalContext.CarryFlag.ToString();
            prcTextBox.Text = GlobalData.Instance.globalContext.ProgrammCounter.ToString();
        }
    }
}
