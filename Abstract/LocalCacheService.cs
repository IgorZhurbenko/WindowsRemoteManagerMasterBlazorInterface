using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Abstract
{
    public class  LocalCacheService : ILocalCacheService
    {
        public string CacheBaseFolder { get; set; }
        public void StoreInfo(string InfoName, string InfoContent)
        {
            if (File.Exists(this.CacheBaseFolder + "\\" + InfoName))
            {
                File.Delete(this.CacheBaseFolder + "\\" + InfoName);
            }
            File.AppendAllText(this.CacheBaseFolder + "\\" + InfoName, InfoContent);
        }

        public string GetStoredInfo(string InfoName)
        {
            if (File.Exists(this.CacheBaseFolder + "\\" + InfoName))
            {
                return File.ReadAllText(this.CacheBaseFolder + "\\" + InfoName);
            }
            else { return "No info of the type stored"; }
        }

    }
}
