using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NodeEditor;
using SampleCommon;

namespace MathSample
{
    // Main context of the sample, each
    // method corresponds to a node by attribute decoration
    public class MathContext : INodesContext
    {
        public NodeVisual CurrentProcessingNode { get; set; }
        public event Action<string, NodeVisual, FeedbackType, object, bool> FeedbackInfo;

        [Node("Value", "Input", "Basic", "Allows to output a simple value.",false)]
        public void InputValue(ushort inValue, out ushort outValue)
        {
            outValue = inValue;
        }

        [Node("AXInValue", "Input", "Basic", "Allows to output a simple value2.", false)]
        public void AXInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.AX = inValue;
        }

        [Node("ЗначВыход", "Выход", "Basic", "Allows to output a simple value.", false, IsOnlyOut = true)]
        public void OutputValue(ushort inValue, out ushort outValue)
        {
            outValue = inValue;
        }

        [Node("РегистрAXВыход", "Выход", "Basic", "Allows to output a simple value1.", false, IsOnlyOut = true)]
        public void AXOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.AX;
        }


        [Node("Add","Operators","Basic","Adds two input values.",false)]
        public void Add(ushort a, ushort b, out ushort result)
        {
            result = (ushort)(a + b);
        }

        [Node("Substract", "Operators", "Basic", "Substracts two input values.", false)]
        public void Substract(ushort a, ushort b, out ushort result)
        {
            result = (ushort)(a - b);
        }

        [Node("Multiply", "Operators", "Basic", "Multiplies two input values.", false)]
        public void Multiplty(ushort a, ushort b, out ushort result)
        {
            result = (ushort)(a * b);
        }

        [Node("Divide", "Operators", "Basic", "Divides two input values.", false)]
        public void Divid(ushort a, ushort b, out ushort result)
        {
            
            result = (ushort)(a % b);
        }

        [Node("Show Value","Helper","Basic","Shows input value in the message box.")]
        public void ShowMessageBox(ushort x)
        {
            GlobalData.Instance.globalContext.AX = x;
            MessageBox.Show(x.ToString(), "Show Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [Node("Starter","Helper","Basic","Starts execution",true,true)]
        public void Starter()
        {
            
        }
    }
}
