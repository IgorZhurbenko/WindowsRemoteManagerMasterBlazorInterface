using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Mail;
using System.Net;
using Newtonsoft;
using System.Dynamic;
using System.Threading.Tasks;
using WindowsRemoteManager.YandexDisk;
using Newtonsoft.Json;

namespace WindowsRemoteManager
{
    public class  WindowsRemoteManagerExecutive : WindowsRemoteManagerGeneral
    {
        private string NickName = "NickName";
        private readonly string ID;
        private readonly string BaseFolder;
        private int ReportingStatusIntervalInMilliseconds = ConstantValues.TimeCriticalValues.ReportStatusIntervalInMilliseconds;

        public WindowsRemoteManagerExecutive(string id, string baseFolder, ICommunicator communicator, ILocalLogger logger, ILocalCacheService cacheService)
            : base(communicator, logger, cacheService)
        {
            this.ID = id;
            this.BaseFolder = baseFolder;
        }

        public WindowsRemoteManagerExecutive() : base(
            new YandexDiskCommunicator(ConstantValues.ConfigurationDefaultInfo.YandexDiskToken, ConstantValues.ConfigurationDefaultInfo.YandexDiskFolder),
            new LocalLogger(),
            new LocalCacheService()
            )
        {
            this.ID = Handle.GetMacAddress().Replace("-", "");
            this.BaseFolder = @$"C:\Users\{Environment.UserName}\appdata\WRME";
        }

        private CommandExecutionResult ExecuteBat(Command command)
        {

            string BatContent = "";

            foreach (string Instruction in command.Instructions)
            {
                if (Instruction.StartsWith("{"))
                {
                    BatContent = BatContent + '\n' + Instruction.Replace("{", "").Replace("}", "");
                }
                else if (Instruction.EndsWith("}") || Instruction.EndsWith("}\r"))
                {
                    BatContent = BatContent + '\n' + Instruction.Replace("}", "").Replace("{", "");
                    break;
                }
                else
                {
                    BatContent = BatContent + '\n' + Instruction;
                }
            }

            string FileName = this.BaseFolder + @"\" + "Command " + this.ID.ToString() + " " + command.ID.ToString() + ".bat";

            if (File.Exists(FileName)) { File.Delete(FileName); }

            File.AppendAllText(FileName, BatContent);

            var result = ProcessRunner.ExecuteCMDCommand(FileName);

            File.Delete(FileName);
            return new CommandExecutionResult { GivenCommandID = command.ID, Output = result };
        }

        private CommandExecutionResult ExecuteCommand(Command command)
        {
            List<string> result = new List<string>();

            if (command.Instructions[0].StartsWith("{"))
            {
                return this.ExecuteBat(command);
            }

            foreach (string Instruction in command.Instructions)
            {
                if (!Instruction.StartsWith(@"'") && !Instruction.StartsWith(@"{"))
                {
                    result.Add(ProcessRunner.ExecuteCMDCommand(Instruction));
                }
                else if (Instruction.StartsWith(@"'curl"))
                {
                    try
                    {
                        result.Add(ProcessRunner.ExecuteCommand("curl.exe", @Instruction.Replace(@"'curl", "")));
                    }
                    catch (Exception error) { result.Add(error.Message); }
                }
                else if (Instruction.ToLower().StartsWith(@"'setrequestsinterval"))
                {
                    string IntervalString = Instruction.Split(" ")[1].Trim();
                    try
                    {
                        int Interval = Convert.ToInt32(IntervalString);
                        if (Interval < 100) { Interval = 100; }
                        if (Interval > 3600000) { Interval = 3600000; }
                        this.RequestsIntervalInMilliseconds = Interval;
                        result.Add("Requests interval set to " + Interval.ToString());
                    }
                    catch { result.Add("Wrong input for requests interval"); }

                }
                else if (Instruction.ToLower().StartsWith(@"'setnickname"))
                {
                    if (Instruction.Split(" ").Length > 1)
                    {
                        this.NickName = Instruction.Split(" ")[1].Trim();
                        result.Add("Nickname of the executive changed to " + this.NickName);
                        Communicator.ReportStatus(ConstantValues.ManagerStatuses.Active);
                    }
                }
            }
            return new CommandExecutionResult()
            {
                GivenCommandID = command.ID,
                Output = string.Join('\n', result)
            };
        }

        private void LoopAction()
        {
            var Commands = this.Communicator.GetCommands();
            foreach (Command command in Commands)
            {
                var commandExecutionResult = this.ExecuteCommand(command);
                new Task(() => this.Communicator.SendCommandExecutionResult(commandExecutionResult)).Start();
                new Task(() => this.Communicator.DeleteMessage($"{command.ID}.{ConstantValues.FileExtensions.CommandInstructions}")).Start();
            }
        }

        public void Launch()
        {
            bool connectionSet = Communicator.CheckConnection();

            if (!connectionSet)
            {
                Logger.Log("Could not set connection.");
                throw new Exception("Could not set connection");
            }

            Logger.Log("Connection successfully set.");

            try
            {
                Communicator.RegisterExecutive(this.ID);
                Logger.Log("Executive registered with ID " + this.ID + ".");
            }
            catch (Exception ex)
            {
                Logger.Log($"Could not register executive. Launch aborted. Error:\n{ex.Message}");
                throw ex;
            }

            var action = new Action<object>(obj => this.Communicator.ReportStatus(ConstantValues.ManagerStatuses.Active));
            var timer = new Timer(new TimerCallback(action), null, 0, this.ReportingStatusIntervalInMilliseconds);

            Logger.Log("Active status reporting initiated.");

            int LoopNumber = 0;
            while (true)
            {
                this.LoopAction();
                LoopNumber++;
                Logger.Log("Loop action number " + LoopNumber.ToString() + " has finished.");
                Thread.Sleep(RequestsIntervalInMilliseconds);
            }
        }

        protected enum RegistrationOption
        {
            Upload,
            Accept
        }
    }
}
