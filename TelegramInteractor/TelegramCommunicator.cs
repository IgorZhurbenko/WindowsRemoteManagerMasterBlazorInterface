using Abstract;
using System;
using System.Collections.Generic;


namespace TelegramCommunicator
{
    public class TelegramCommunicator : ICommunicator
    {
        public string AttachedExecutiveID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckConnection()
        {
            throw new NotImplementedException();
        }

        public void DeleteMessage(string MessageTitle)
        {
            throw new NotImplementedException();
        }

        public List<ExecutiveInfo> GetAllExecutives()
        {
            throw new NotImplementedException();
        }

        public CommandExecutionResult GetCommandExecutionResult(string commandID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Command> GetCommands()
        {
            throw new NotImplementedException();
        }

        public ExecutiveStatus GetExecutiveStatus(string ExecutiveID = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterExecutive(string id)
        {
            throw new NotImplementedException();
        }

        public void ReportStatus(string StatusName)
        {
            throw new NotImplementedException();
        }

        public bool SendCommand(Command command)
        {
            throw new NotImplementedException();
        }

        public void SendCommandExecutionResult(CommandExecutionResult commandExecutionResult)
        {
            throw new NotImplementedException();
        }
    }
}
