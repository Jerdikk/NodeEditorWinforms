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

        [Node("Value", "Input", "Basic", "Allows to output a simple value.", IsCallable = false)]
        public void InputValue(ushort inValue, out ushort outValue)
        {
            outValue = inValue;
        }

        [Node("РегистрAккВход", "Input", "Basic", "Allows to in to Accumulator.", IsCallable = false)]
        public void AccumulatorInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.Accumulator = (byte)inValue;
        }

        [Node("РегистрСчКоммВход", "Input", "Basic", "Allows to in to Accumulator.", IsCallable = false)]
        public void ProgramCounterInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.ProgrammCounter = (byte)inValue;
        }

        [Node("ЗначПамятиВыход", "Выход", "Basic", "Allows to output a simple value.", IsCallable = false, IsOnlyOut = true)]
        public void MemoryOutputValue(ushort inValue, out ushort outValue)
        {
            inValue &= 255;
            outValue = GlobalData.Instance.globalContext.Memory[inValue];
        }

        [Node("ЗначВыход", "Выход", "Basic", "Allows to output a simple value.", IsCallable = false, IsOnlyOut = true)]
        public void OutputValue(ushort inValue, out ushort outValue)
        {
            outValue = inValue;
        }

        [Node("РегистрAккВыход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void AccumulatorOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.Accumulator;
        }
        [Node("РегистрСчКоммВыход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void ProgramCounterOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.ProgrammCounter;
        }

        [Node("ПамятьКомандВыход", "Выход", "Basic", "Allows to output value from code memory.", IsCallable = false, IsOnlyOut = true)]
        public void ProgramMemoryOutputValue1(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.Code[GlobalData.Instance.globalContext.ProgrammCounter];
        }
        [Node("ПамятьКомандВыход+1", "Выход", "Basic", "Allows to output value from code memory.", IsCallable = false, IsOnlyOut = true)]
        public void ProgramMemoryOutputValue2(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.Code[GlobalData.Instance.globalContext.ProgrammCounter+1];
        }
        [Node("ПамятьКомандВыход+2", "Выход", "Basic", "Allows to output value from code memory.", IsCallable = false, IsOnlyOut = true)]
        public void ProgramMemoryOutputValue3(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.Code[GlobalData.Instance.globalContext.ProgrammCounter+2];
        }

        [Node("Add","Operators","Basic","Adds two input values.",IsCallable = false)]
        public void Add(ushort a, ushort b, out ushort result)
        {
            result = (ushort)(a + b);
        }

        [Node("Substract", "Operators", "Basic", "Substracts two input values.", IsCallable = false)]
        public void Substract(ushort a, ushort b, out ushort result)
        {
            result = (ushort)(a - b);
        }

        [Node("Multiply", "Operators", "Basic", "Multiplies two input values.", IsCallable = false)]
        public void Multiplty(ushort a, ushort b, out ushort result)
        {
            result = (ushort)(a * b);
        }

        [Node("Divide", "Operators", "Basic", "Divides two input values.", IsCallable = false)]
        public void Divid(ushort a, ushort b, out ushort result)
        {
            
            result = (ushort)(a % b);
        }

        [Node("Show Value","Helper","Basic","Shows input value in the message box.")]
        public void ShowMessageBox(ushort x)
        {
            GlobalData.Instance.globalContext.Accumulator = (byte)x;
            MessageBox.Show(x.ToString(), "Show Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [Node("Run node", "Helper", "Basic", "Runs one step.",OutExec =2)]
        public void RunNode(ushort Acc, ushort PrC)
        {
            GlobalData.Instance.globalContext.Accumulator = (byte)Acc;           
            GlobalData.Instance.globalContext.ProgrammCounter = (byte)PrC;
        }

        [Node("Starter","Helper","Basic","Starts execution",true,true)]
        public void Starter()
        {
            
        }
    }
}
