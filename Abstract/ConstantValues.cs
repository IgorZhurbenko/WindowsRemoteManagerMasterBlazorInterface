using System;
using System.Collections.Generic;
using System.Text;

namespace Abstract
{
    public static class ConstantValues
    {
        public static class ManagerStatuses
        {
            public static string Active = "Active";
            public static string Off = "Off";
        }

        public static class LogMessages
        {
            public static string CouldNotSendStatus = "Could not report status";
        }

        public static class FileExtensions
        {
            public const string CommandExecutionResult = "res";
            public const string CommandInstructions = "com";
            public const string Status = "stat";
        }

        public static class TimeCriticalValues
        {
            public const int MillisecondsBeforeConsideredInactive = 30000;
            public const int IntervalBetweenExecutiveRequests = 1000;
            public const int ReportStatusIntervalInMilliseconds = 15000;
        }

        public static class ConfigurationDefaultInfo
        {
            public const string YandexDiskToken = "AgAAAAA8iGE8AAY9JguHKCi7Ak8qt9z79ePI6PM";
            public const string YandexDiskFolder = "WindowsRemoteManager";
        }
    }
}
