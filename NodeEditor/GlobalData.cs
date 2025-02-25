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
        /* public byte AH;
         public byte AL;

         public ushort AX
         {
             get
             {
                 return (ushort)(AH * 256 + AL);
             }
             set
             {
                 AL = (byte)(value & 0xF);
                 AH = (byte)(value >> 8);
             }
         }*/
        // # accumulator
        public byte Accumulator;
        // # program counter
        public byte ProgrammCounter;
        // # carry flag
        public byte CarryFlag;

        //public byte CodePointer;
        //public byte MemoryPointer;

        public byte[] Memory = new byte[256];
        public byte[] Code = new byte[256];

        public string Instruction;

        public GlobalContext()
        {
            Memory = new byte[256];
            Code = new byte[256];
            Instruction = "";
           // CodePointer = 0;
          //  MemoryPointer = 0;
           // AX = 0;
        }

        public void Reset()
        {
            Memory = new byte[256];
            Code = new byte[256];
            Instruction = "";
            Accumulator = 0;
            ProgrammCounter = 0;
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
