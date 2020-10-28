using System;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using Abstract;

namespace RemoteManagerMaster
{

    public class RemoteManagerMaster : WindowsRemoteManagerGeneral
    {
        public IUserInteractor UserInteractor;
        public RemoteManagerMaster(ICommunicator communicator, ILocalLogger logger, ILocalCacheService cacheService)
            : base(communicator, logger, cacheService)
        {
            //this.UserInteractor = userInteractor;
        }

        //public WindowsRemoteManagerMaster() : 
        //    base(
        //        new YandexDiskCommunicator(ConstantValues.ConfigurationDefaultInfo.YandexDiskToken, ConstantValues.ConfigurationDefaultInfo.YandexDiskFolder),
        //        new LocalLogger(), 
        //        new LocalCacheService()
        //        )
        //{
            
        //}
        //private string RecordInstruction()
        //{
        //    string EnteredLine;
        //    bool RecordingBat = false;
        //    string instruction = "";

        //    do
        //    {
        //        EnteredLine = Console.ReadLine();
        //        RecordingBat = (RecordingBat || EnteredLine.StartsWith("{")) && !EnteredLine.EndsWith("}");
        //        instruction = instruction + EnteredLine + '\n';
        //    }
        //    while (RecordingBat);

        //    return instruction;
        //}

        //private bool LoopAction(string Instruction)
        //{

        //    try
        //    {
        //        string HashCode = Instruction.GetHashCode().ToString();

        //        this.SendMessage("Command " + this.ID.ToString() + " " + HashCode, Instruction);

        //        Thread.Sleep(RequestsInterval + 200);
        //        string report = "";
        //        do
        //        { report = GetReport(HashCode); Thread.Sleep(RequestsInterval + 100); }
        //        while (report.StartsWith("Error"));

        //        Console.WriteLine(report);

        //        return true;
        //    }
        //    catch { return LoopAction(Instruction); }

        //}

        
        //public void Launch()
        //{
        //    this.yandexDiskManager.Token = this.GetStoredInfo("YandexDiskToken");

        //    while (this.yandexDiskManager.Token == "No info of the type stored" || !this.CheckConnection())
        //    {
        //        if (this.yandexDiskManager.Token == "No info of the type stored")
        //        {
        //            Console.Write("Yandex disk token not found. Type valid token here: ");
        //        }
        //        else { Console.Write("Connection check by the given Yandex disk token failed. Type valid token here: "); }
        //        this.yandexDiskManager.Token = Console.ReadLine();
        //    }

        //    this.StoreInfo("YandexDiskToken", this.yandexDiskManager.Token);

        //    List<string> RegisteredExecutives = this.GetAllExecutives();

        //    if (!(RegisteredExecutives.Count < 1))
        //    {
        //        Console.WriteLine("Number|ID|NickName|DateOfLastAction|Status");
        //        for (int a = 1; a <= RegisteredExecutives.Count; a++)
        //        { Console.WriteLine(a.ToString() + '|' + RegisteredExecutives[a - 1]); }
        //        Console.WriteLine("Connection set. Here is the table of registered executives. Choose ID of the one you want to manage.");
        //    }

        //    else
        //    {
        //        Console.WriteLine("There are no registered executives. It is unlikely that any executive is operational now\n " +
        //            "You may still enter an ID");
        //    }

        //    bool error = true;
        //    while (error)
        //    {
        //        try
        //        {
        //            this.ID = RegisteredExecutives[Convert.ToInt32(Console.ReadLine()) - 1].Split('|')[0];
        //            this.YaDiskBaseFolder = this.YaDiskBaseFolder + "/" + this.ID;
        //            error = false;
        //        }
        //        catch { Console.WriteLine("Invalid value entered. Try only numbers this time."); }
        //    }
        //    Task.Run(() => ClearAllReports());

        //    int i = 1;

        //    while (true)
        //    {
        //        string instruction = RecordInstruction();
        //        LoopAction(instruction);
        //        i++;
        //    }
        //}


        //public string GetExecutiveStatus(string ExecutivesID, int CriticalTimeInSeconds = 75)
        //{
        //    string result = "Inactive";
        //    List<Task> DeleteOldStatusesTasks = new List<Task>();
        //    var FileStructure = this.yandexDiskManager.GetFileStructure(this.YaDiskBaseFolder + "/" + ExecutivesID);
        //    foreach (Dictionary<string, string> file in FileStructure)
        //    {
        //        if (file["name"].StartsWith("Active_")) 
        //        {
        //            DateTime date;
        //            if (DateTime.TryParse(file["name"].Split('_')[2].Replace('z',':'), out date)
        //                &&
        //                date.CompareTo(DateTime.Now.AddSeconds(-CriticalTimeInSeconds)) > 0
        //                )
        //            {
        //                result = "Active";
        //                break;
        //            }
        //            else 
        //            { 
        //                Task DeleteTask = new Task(() => this.yandexDiskManager.DeleteFile(this.YaDiskBaseFolder + "/" + ExecutivesID + "/" + file["name"]));
        //                DeleteTask.Start();
        //                DeleteOldStatusesTasks.Add(DeleteTask);
        //            }
        //        }
        //    }
        //    Task.WaitAll(DeleteOldStatusesTasks.ToArray());
        //    return result;
        //}

        //private string GetReport(string CommandID)
        //{
        //    try
        //    {
        //        return this.GetMessage("Report " + this.ID.ToString() + " " + CommandID);
        //    }
        //    catch
        //    {
        //        return "Error: message not acquired";
        //    }
        //}


    }

}

