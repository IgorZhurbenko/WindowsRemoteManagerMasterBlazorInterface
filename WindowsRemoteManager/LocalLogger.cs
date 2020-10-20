using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WindowsRemoteManager.YandexDisk
{
    public class  LocalLogger : ILocalLogger
    {
        public string LogFilePath { get; set; }
        public bool DisplayOnConsole { get; set; }

        public LocalLogger(string logFilePath = null, bool displayOnConsole = true)
        {
            this.LogFilePath = logFilePath ?? @$"C:\Users\{Environment.UserName}\appdata\local\WRME\log.txt";
            this.DisplayOnConsole = displayOnConsole;
        }
        
        public void Log(string Text)
        {
            string messageToWrite = DateTime.Now.ToString() + ": " + Text;
            try { File.AppendAllText(this.LogFilePath, messageToWrite); }
            catch (IOException ex)
            {
                System.Threading.Thread.Sleep(500);
                Log(Text);
            }
            if (DisplayOnConsole)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": " + Text);
            }
        }

        public void LogEmptyLine()
        {
            try { File.AppendAllText(this.LogFilePath, "\n"); }
            catch { }
            Console.WriteLine();
        }

    }
}
