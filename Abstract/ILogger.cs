using System;
using System.Threading.Tasks;

namespace Abstract
{
    public interface ILocalLogger
    {
        public string LogFilePath { get; set; }
        public bool DisplayOnConsole { get; set; }
        public void Log(string text);
        public void LogEmptyLine();
    }

    public interface IWebLogger
    {
        public string WebLogFilePath { get; set; }

        public async Task<bool> LogToWeb(string Text) 
        {
            throw new NotImplementedException();
        }

        public bool CheckConnection();
    }
}