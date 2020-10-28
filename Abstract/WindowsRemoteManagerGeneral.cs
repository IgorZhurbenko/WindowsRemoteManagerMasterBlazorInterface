using System;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Diagnostics;
using WindowsRemoteManager.YandexDisk;

namespace Abstract
{
    public abstract class WindowsRemoteManagerGeneral
    {
        protected ICommunicator Communicator;
        protected ILocalLogger Logger;
        protected ILocalCacheService LocalCacheService;
        protected int RequestsIntervalInMilliseconds = ConstantValues.TimeCriticalValues.IntervalBetweenExecutiveRequests;
        

        public WindowsRemoteManagerGeneral(
            ICommunicator communicator, 
            ILocalLogger logger, 
            ILocalCacheService cacheService
            )
        {
            this.Communicator = communicator;
            this.LocalCacheService = cacheService;
            this.Logger = logger;
        }

    }
}

