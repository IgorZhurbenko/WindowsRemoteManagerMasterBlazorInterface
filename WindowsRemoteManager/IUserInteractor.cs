using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsRemoteManager.YandexDisk
{
    public interface IUserInteractor
    {
        public Command RecordNewCommand();
        public string RecordNewInstructions();
    }
}
