using System;
using WindowsRemoteManager;

namespace WRMExecutiveConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var WRME = new WindowsRemoteManagerExecutive();

            WRME.Launch();
        }
    }
}
