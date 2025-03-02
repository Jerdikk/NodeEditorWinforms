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

        [Node("Value", "Вход", "Basic", "Allows to output a simple value.", IsCallable = false)]
        public void InputValue(ushort inValue, out ushort outValue)
        {
            outValue = inValue;
        }

        [Node("ПамятьВход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void MemoryInputValue(ushort addr, ushort inValue)
        {
            GlobalData.Instance.globalContext.Memory[addr] = (byte)inValue;
        }


        [Node("РегистрAккВход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void AccumulatorInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.A = (byte)inValue;
        }

        [Node("РегB_Вход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void RegBInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.B = (byte)inValue;
        }
        [Node("РегC_Вход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void RegCInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.C = (byte)inValue;
        }
        [Node("РегD_Вход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void RegDInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.D = (byte)inValue;
        }
        [Node("РегE_Вход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void RegEInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.E = (byte)inValue;
        }
        [Node("РегH_Вход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void RegHInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.H = (byte)inValue;
        }
        [Node("РегL_Вход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void RegLInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.L = (byte)inValue;
        }


        [Node("РегистрФлагВход", "Вход", "Basic", "Allows to in to A.", IsCallable = false)]
        public void CarryInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.Flags = (byte)inValue;
        }

        [Node("РегистрСчКоммВход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void ProgramCounterInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.ProgrammCounter = (ushort)inValue;
        }

        

        [Node("УказательСтекаВход", "Вход", "Basic", "Allows to in to A.", IsCallable = true)]
        public void StackPointerInputValue(ushort inValue)
        {
            GlobalData.Instance.globalContext.StackPointer = (ushort)inValue;
        }

        [Node("ЗначПамятиВыход", "Выход", "Basic", "Allows to output a simple value.", IsCallable = false, IsOnlyOut = false)]
        public void MemoryOutputValue(ushort inValue, out ushort outValue)
        {
            try
            {
                inValue &= 65535;
                outValue = GlobalData.Instance.globalContext.Memory[inValue];
            }
            catch (Exception e)
            {
                outValue = 0;
                MessageBox.Show(e.Message);
            }
        }

        [Node("ЗначВыход", "Выход", "Basic", "Allows to output a simple value.", IsCallable = false, IsOnlyOut = true/*, CustomEditor = typeof(NumericUpDown)*/)]
        public void OutputValue(ushort inValue, out ushort outValue)
        {
            outValue = inValue;
        }

        [Node("РегистрAккВыход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void AccumulatorOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.A;
        }

        [Node("РегистрB_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegBOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.B;
        }
        [Node("РегистрC_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegCOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.C;
        }
        [Node("РегистрD_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegDOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.D;
        }
        [Node("РегистрE_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegEOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.E;
        }
        [Node("РегистрH_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegHOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.H;
        }
        [Node("РегистрL_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegLOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.L;
        }
        [Node("РегистрBC_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegBCOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.BC;
        }
        [Node("РегистрDE_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegDEOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.DE;
        }
        [Node("РегистрHL_Выход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void RegHLOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.HL;
        }



        [Node("РегистрФлагВыход", "Выход", "Basic", "Allows to output a simple value1.", IsCallable = false, IsOnlyOut = true)]
        public void CarryOutputValue(out ushort outValue)
        {
            outValue = GlobalData.Instance.globalContext.Flags;
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

        [Node(">", "Operators", "Basic", "Divides two input values.", IsCallable = false)]
        public void More(ushort a, ushort b, out bool result)
        {

            result = a > b;
        }
        [Node("<", "Operators", "Basic", "Divides two input values.", IsCallable = false)]
        public void Less(ushort a, ushort b, out bool result)
        {

            result = a < b;
        }

        [Node("==", "Operators", "Basic", "Divides two input values.", IsCallable = false)]
        public void Eequal(ushort a, ushort b, out bool result)
        {

            result = a == b;
        }

        [Node("Not", "Operators", "Basic", "Divides two input values.", IsCallable = false)]
        public void Enot(bool a, out bool result)
        {

            result = !a;
        }

        [Node("Show Value","Helper","Basic","Shows input value in the message box.")]
        public void ShowMessageBox(ushort x)
        {
            GlobalData.Instance.globalContext.A = (byte)x;
            MessageBox.Show(x.ToString(), "Show Value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [Node("Run node", "Helper", "Basic", "Runs one step.")]
        public void RunNode(ushort Acc, ushort PrC)
        {
            GlobalData.Instance.globalContext.A = (byte)Acc;           
            GlobalData.Instance.globalContext.ProgrammCounter = (byte)PrC;
        }

        [Node("Bool node", "Helper", "Basic", "Runs one step.", OutExec = 2)]
        public void BoolNode(bool condition)
        {
            
        }



        [Node("Starter","Helper","Basic","Starts execution",true,true)]
        public void Starter()
        {
            
        }
    }
}
