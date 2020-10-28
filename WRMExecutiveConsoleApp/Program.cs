using System;


namespace WindowsRemoteManagerExecutiveConsoleInterface
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
