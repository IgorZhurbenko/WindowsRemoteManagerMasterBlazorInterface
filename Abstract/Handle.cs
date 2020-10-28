using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
namespace Abstract
{
    static class Handle
    {
        public static string FormatDateForFileName(this DateTime date)
        {
            return date.ToString().Replace(':', 'z');
        }

        public static DateTime DeformatDateFromFileName(this string formattedDate)
        {
            return DateTime.Parse(formattedDate.Replace('z', ':'));
        }

        public static DateTime TakeDateOutOfFileName(this string fileName)
        {
            return fileName.Split('_')[1].Split('.')[0].DeformatDateFromFileName();
        }

        public static bool TryForBool(this Func<bool> actionFunc)
        {
            try { return actionFunc.Invoke(); }
            catch { return false; }
        }

        public static object DoOrErrorMessage<T>(Func<T> func)
        {
            try { return func.Invoke(); }
            catch (Exception ex) { return ex.Message; }
        }

        public static string GetMacAddress()
        {
            var res = ProcessRunner.ExecuteCMDCommand("getmac");
            return res.Split('\n')[3].Split(" ")[0];
        }
    }
}
