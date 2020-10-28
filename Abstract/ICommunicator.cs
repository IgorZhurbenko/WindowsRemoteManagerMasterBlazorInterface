using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Abstract
{
    public interface ICommunicator
    {
        public string AttachedExecutiveID { get; set; }
        public IEnumerable<Command> GetCommands();
        public async Task<CommandExecutionResult> SendCommandGetResponseAsync(Command command)
        {
            throw new NotImplementedException();
        }
        public CommandExecutionResult GetCommandExecutionResult(string commandID);
        public void SendCommandExecutionResult(CommandExecutionResult commandExecutionResult);
        public void DeleteMessage(string MessageTitle);
        public bool SendCommand(Command command);
        public bool CheckConnection();
        public ExecutiveStatus GetExecutiveStatus(string ExecutiveID = null);
        public void ReportStatus(string StatusName);
        public void RegisterExecutive(string id);
        public List<ExecutiveInfo> GetAllExecutives();
    }


}
