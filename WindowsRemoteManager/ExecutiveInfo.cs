using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsRemoteManager.YandexDisk
{
    public class ExecutiveInfo
    {
        public string ID;
        public DateTime LastReported;
        public string CommonName;
        public string Status;
        public bool IsActive
        {
            get
            {
                return DateTime.Now.Subtract(LastReported).TotalMilliseconds < ConstantValues.TimeCriticalValues.MillisecondsBeforeConsideredInactive;
            }
        }
    }
}
