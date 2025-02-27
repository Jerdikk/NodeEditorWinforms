using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace SampleCommon
{
    [Serializable]
    public class GlobalContext
    {
        // # accumulator
        public byte A;

        public byte B;
        public byte C;

        public ushort BC
        {
            get
            {
                return (ushort)(B * 256 + C);
            }
            set
            {
                C = (byte)(value & 0xF);
                B = (byte)(value >> 8);
            }
        }
        public byte D;
        public byte E;

        public ushort DE
        {
            get
            {
                return (ushort)(D * 256 + E);
            }
            set
            {
                E = (byte)(value & 0xF);
                D = (byte)(value >> 8);
            }
        }
        public byte H;
        public byte L;

        public ushort PSW
        {
            get
            {
                return (ushort)(A * 256 + Flags);
            }
            set
            {
                Flags = (byte)(value & 0xF);
                A = (byte)(value >> 8);
            }
        }

        public ushort HL
        {
            get
            {
                return (ushort)(H * 256 + L);
            }
            set
            {
                L = (byte)(value & 0xF);
                H = (byte)(value >> 8);
            }
        }

        public bool BitZ; //zero
        public bool BitS; //negative value
        public bool BitP; // parity bits
        public bool BitC; // carry
        public bool BitAC; // 4bit carry

        // # program counter
        public ushort ProgrammCounter;
        public ushort StackPointer;
        // # carry flag
        public byte Flags
        {
            get
            {
                int t1 = (BitC ? 1 : 0) + 2;
                t1 += (BitP ? 4 : 0);                
                t1 += (BitAC ? 16 : 0);
                t1 += (BitZ ? 64 : 0);
                t1 += (BitS ? 128 : 0);
                return (byte)(t1 & 255);
            }
            set
            {
                byte t2 = (byte)(value & 255);
                BitS = ((t2 & 0b10000000) == 128);
                BitZ = ((t2 & 0b1000000) == 64);
                BitAC = ((t2 & 0b10000) == 16);
                BitP = ((t2 & 0b100) == 4);
                BitC = ((t2 & 0b1) == 1);
            }
        }

        //public byte CodePointer;
        //public byte MemoryPointer;

        public byte[] Memory;// = new byte[65535];
        public byte[] Code;// = new byte[65535];

        public string Instruction;

        public GlobalContext()
        {
            Memory = new byte[65535];
            Code = new byte[65535];
            Instruction = "";
            ProgrammCounter = 0;
            StackPointer = 0;
          //  Flags = 255;
            int yy = 1;

            // CodePointer = 0;
            //  MemoryPointer = 0;
            // AX = 0;
        }

        public void Reset()
        {
            Memory = new byte[65535];
            Code = new byte[65535];
            Instruction = "";
            A = 0;
            ProgrammCounter = 0;
            StackPointer = 0;
            //  AX = 0;
        }
    }
    public sealed class GlobalData
    {
        #region Singleton
        private static object syncRoot = new Object();
        private static volatile GlobalData instance;
        public static int programNum = 0;
        public static string mutexString;
        public static Mutex m = null;

        public static GlobalData Instance
        {
            get
            {
                if (instance == null)
                {

                    bool doesNotExist = false;
                    bool otherException = false;
                    bool createdMutex = false;

                    while (!createdMutex)
                    {
                        mutexString = "NodeForms" + programNum.ToString();
                        try
                        {
                            // Open the mutex with (MutexRights.Synchronize |
                            // MutexRights.Modify), to enter and release the
                            // named mutex.
                            //                                                       

                            m = Mutex.OpenExisting(mutexString);


                        }
                        catch (WaitHandleCannotBeOpenedException)
                        {
                            Console.WriteLine("Mutex does not exist.");
                            doesNotExist = true;
                            m = new Mutex(false, mutexString, out createdMutex);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception in mutex: {0}", ex.Message);
                            otherException = true;
                        }

                        if ((m != null) && (!doesNotExist) && (!otherException))
                        {
                            // m.ReleaseMutex();
                            m = null;
                            programNum++;
                        }

                    }


                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new GlobalData();
                    }
                }

                return instance;
            }
        }
        #endregion



        public GlobalContext globalContext = null;

        public GlobalData()
        {
            globalContext = new GlobalContext();
        }

        public void SerializeContext()
        {
            try
            {
                String xmlPath = AppDomain.CurrentDomain.BaseDirectory;
                String XMLFileName = xmlPath + "\\global_context.xml";//Environment.CurrentDirectory + "\\settings.xml";
                XmlSerializer formatter = new XmlSerializer(typeof(GlobalContext));
                using (FileStream fs = new FileStream(XMLFileName, FileMode.Create))
                {
                    formatter.Serialize(fs, globalContext);//Points_to_Save
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " -- " + e.StackTrace);
            }
        }

        public void DeserializeContext()
        {
            try
            {
                GlobalContext Localsettings = new GlobalContext();
                String xmlPath = AppDomain.CurrentDomain.BaseDirectory;//Assembly.GetExecutingAssembly().Location;//Environment.CurrentDirectory;
                String XMLFileName = xmlPath + "global_context.xml";//Environment.CurrentDirectory + "\\settings.xml";
                XmlSerializer formatter = new XmlSerializer(typeof(GlobalContext));
                if (File.Exists(XMLFileName))
                {
                    try
                    {
                        using (FileStream fstream = new FileStream(XMLFileName, FileMode.Open))
                        {
                            XmlReader reader = new XmlTextReader(fstream);
                            if (formatter.CanDeserialize(reader))
                            {
                                Localsettings = formatter.Deserialize(reader) as GlobalContext;
                            }
                            // fstream.Dispose();// Close();
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " -- " + e.StackTrace);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " -- " + e.StackTrace);
            }
        }

    }
}
