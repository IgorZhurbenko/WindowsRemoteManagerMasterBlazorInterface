using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Abstract
{
    public static class ProcessRunner
    {
        public static string ExecuteCMDCommand(string command)
        {
            ProcessStartInfo psiOpt = new ProcessStartInfo("cmd.exe", "/C" + command);
            psiOpt.WindowStyle = ProcessWindowStyle.Hidden;
            psiOpt.RedirectStandardOutput = true;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;
            Process procCommand = Process.Start(psiOpt);
            StreamReader sr = procCommand.StandardOutput;
            var result = sr.ReadToEnd();
            return result;
        }

        public static string ExecuteCommand(string app, string runParameters)
        {
            ProcessStartInfo psiOpt = new ProcessStartInfo(app, "/C" + runParameters);
            psiOpt.WindowStyle = ProcessWindowStyle.Hidden;
            psiOpt.RedirectStandardOutput = true;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;
            Process procCommand = Process.Start(psiOpt);
            StreamReader sr = procCommand.StandardOutput;
            var result = sr.ReadToEnd();
            return result;
        }
    }
}
