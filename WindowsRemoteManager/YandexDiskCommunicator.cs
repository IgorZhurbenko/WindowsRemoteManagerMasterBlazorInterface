using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsRemoteManager.YandexDisk;
using System.Diagnostics;
using Newtonsoft.Json;

namespace WindowsRemoteManager
{
    public class  YandexDiskCommunicator : ICommunicator
    {
        private YandexDiskManager yandexDiskManager;
        public string AttachedExecutiveID { get; set; }

        public YandexDiskCommunicator(string yandexDiskToken, string yandexDiskBaseFolder, string computerBasePath = null, string attachedExecutiveID = null)
        {
            //this.AttachedExecutiveID = attachedExecutiveID;
            this.AttachedExecutiveID = attachedExecutiveID ?? Handle.GetMacAddress().ToString().Replace("-", "");
            this.yandexDiskManager = new YandexDiskManager(yandexDiskToken, yandexDiskBaseFolder, computerBasePath);
        }

        public bool CheckConnection()
        {
            return Handle.TryForBool(() => this.yandexDiskManager.CheckConnection());
        }

        public IEnumerable<Command> GetCommands()
        {
            List<Command> Commands = new List<Command>();
            var Structure = this.yandexDiskManager.GetFileStructure();

            IEnumerable<Command> commands = Structure.Where(file => file.Name.EndsWith($".{ConstantValues.FileExtensions.CommandInstructions}"))
                .Select(file => new Command()
                {
                    ID = file.Name.Split(".")[0]
                });

            var getInstructionsTasks = new List<Task>();

            List<Command> result = new List<Command>();

            foreach (var command in commands)
            {
                var newCommand = new Command() { ID = command.ID };
                result.Add(newCommand);
                var task = new Task(() =>
                {
                    var instructionsMessage = $"{command.ID}.{ConstantValues.FileExtensions.CommandInstructions}";
                    try
                    {
                        var instructionsList = this.GetMessage(instructionsMessage);
                        newCommand.Instructions = instructionsList.Split('\n').ToList();
                    }
                    catch (FileNotFoundException ex) { newCommand.Instructions = new List<string>() { "echo \"Instructions not retrieved\"\n" }; }
                });
                task.Start();
                Thread.Sleep(5);
                getInstructionsTasks.Add(task);
            }
            Task.WaitAll(getInstructionsTasks.ToArray());
            return result;
        }

        private string GetMessage(string yandexDiskMessageName)
        {
            return this.yandexDiskManager.ReadFileFromYandexDisk(yandexDiskMessageName);
        }

        public Command GetCommand(string CommandID)
        {
            throw new NotImplementedException();
        }

        public ExecutiveStatus GetExecutiveStatus(string ExecutiveID = null)
        {
            throw new NotImplementedException();
        }

        public CommandExecutionResult GetCommandExecutionResult(string commandID)
        {
            return new CommandExecutionResult()
            {
                GivenCommandID = commandID,
                Output = GetMessage($@"{commandID}.{ConstantValues.FileExtensions.CommandExecutionResult}")
            };
        }

        public bool SendCommand(Command command)
        {
            throw new NotImplementedException();
        }

        public void SendCommandExecutionResult(CommandExecutionResult commandExecutionResult)
        {
            try
            {
                this.yandexDiskManager.UploadFileWithContent($"{commandExecutionResult.GivenCommandID}.{ConstantValues.FileExtensions.CommandExecutionResult}", commandExecutionResult.Output);
            }
            catch (FileAlreadyExistsException) { }
        }

        public void ReportStatus(string statusName)
        {
            string activeMessageTitle = $"{statusName}_{DateTime.UtcNow.FormatDateForFileName()}.{ConstantValues.FileExtensions.Status}";

            var fileStructure = this.yandexDiskManager.GetFileStructure();

            List<Task> TaskList = new List<Task>();

            foreach (var file in fileStructure)
            {
                if (file.Name.EndsWith("." + ConstantValues.FileExtensions.Status))
                {
                    Task task = new Task(() => this.yandexDiskManager.DeleteFile(file.Name));
                    task.Start();
                    TaskList.Add(task);
                }
            }
            this.yandexDiskManager.UploadFileWithContent(activeMessageTitle);
            Task.WaitAll(TaskList.ToArray());
        }

        private void ReportActiveStatusRepeatedly(int IntervalInSeconds, ref string logMessage)
        {
            try { ReportStatus(ConstantValues.ManagerStatuses.Active); }
            catch (Exception ex)
            {
                logMessage += $"\n{ConstantValues.LogMessages.CouldNotSendStatus}\nError: {ex.Message}\n\n";
                Thread.Sleep(1000);
                ReportStatus(ConstantValues.ManagerStatuses.Active);
            }
            Thread.Sleep(IntervalInSeconds * 1000);
            ReportActiveStatusRepeatedly(IntervalInSeconds, ref logMessage);
        }

        public void DeleteMessage(string messageTitle)
        {
            this.yandexDiskManager.DeleteFile(messageTitle);
        }

        public void RegisterExecutive(string executiveID)
        {
            this.yandexDiskManager.CreateFolder(executiveID);
            this.yandexDiskManager.YandexDiskBaseFolder += $"/{executiveID}";
        }

        public List<ExecutiveInfo> GetAllExecutives()
        {
            List<YandexDiskFileModel> str = this.yandexDiskManager.GetFileStructure();

            List<ExecutiveInfo> ExecutivesList = new List<ExecutiveInfo>();

            List<Task> GetMessagesTasks = new List<Task>();

            foreach (var ExecutiveFile in str)
            {
                if (ExecutiveFile.Type.ToLower() != "file")
                {
                    ExecutiveInfo executiveInfo = null;
                    ExecutivesList.Add(executiveInfo);
                    Task task = new Task(
                        () =>
                        {
                            string infoMessage = GetMessage(ExecutiveFile.Name + "/" + "Info");

                            var statusInfo = yandexDiskManager.GetFileStructure(ExecutiveFile.Name).Where(file => file.Name.EndsWith(ConstantValues.FileExtensions.Status))
                            .Select(
                                file => new
                                {
                                    StatusName = file.Name.Split('_')[0],
                                    Date = file.Name.TakeDateOutOfFileName()
                                }
                            ).OrderByDescending(statusInfo => statusInfo.Date).FirstOrDefault();

                            executiveInfo = JsonConvert.DeserializeObject<ExecutiveInfo>(infoMessage);

                            executiveInfo.ID = ExecutiveFile.Name;
                            executiveInfo.LastReported = statusInfo.Date;
                            executiveInfo.Status = statusInfo.StatusName;
                        }
                    );
                    task.Start();
                    GetMessagesTasks.Add(task);
                }
            }
            Task.WaitAll(GetMessagesTasks.ToArray());
            return ExecutivesList;
        }

    }
}
