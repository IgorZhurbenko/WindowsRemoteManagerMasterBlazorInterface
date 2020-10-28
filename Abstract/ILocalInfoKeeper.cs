namespace Abstract
{
    public interface ILocalCacheService
    {
        public string CacheBaseFolder { get; set; }
        public void StoreInfo(string InfoName, string InfoContent);
        public string GetStoredInfo(string InfoName);       
    }
}