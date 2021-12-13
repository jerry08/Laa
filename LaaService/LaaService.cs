using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LaaService
{
    partial class LaaService : ServiceBase
    {
        public LaaService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Starting service...");

            try
            {
                string path = @"C:\Users\Jeremy\source\repos\Xamarin Projects\Laa\LaaServer\bin\Debug\net5.0-windows\LaaServer.exe";
                if (System.IO.File.Exists(path))
                    Process.Start(path);
            }
            catch (Exception e)
            {
                WriteToFile($"{e}");
            }

            WriteToFile("Service started");
        }

        protected override void OnStop()
        {
            
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
