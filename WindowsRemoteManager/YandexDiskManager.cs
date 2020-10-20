using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using WindowsRemoteManager.YandexDisk;
using System.Net.Mime;
using Newtonsoft.Json;

namespace WindowsRemoteManager
{
    public class  YandexDiskOperationException : Exception
    {
    }

    public class  FileAlreadyExistsException : YandexDiskOperationException { }
    public class  FileNotFoundException : YandexDiskOperationException { }


    public class  YandexDiskManager
    {
        private string Token;
        private HttpClient httpClient;
        private WebClient webClient;
        private string ComputerBasePath;
        public string YandexDiskBaseFolder;

        public YandexDiskManager(string Token, string yandexDiskBaseFolder, string ComputerBasePath = null)
        {
            this.Token = Token;
            this.ComputerBasePath = ComputerBasePath ?? $@"c:\users\{Environment.UserName}\appdata\local\wrme";
            this.YandexDiskBaseFolder = yandexDiskBaseFolder;
            this.httpClient = new HttpClient();
            this.webClient = new WebClient();
        }

        public bool CheckConnection()
        {
            using (var request = new HttpRequestMessage(new HttpMethod("Get"),
                $@"https://cloud-api.yandex.net/v1/disk/resources?path=/&fields=_embedded.items.name,_embedded.items.type&limit=100"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);
                return httpClient.SendAsync(request).Result.IsSuccessStatusCode;
            }
        }

        public List<YandexDiskFileModel> GetFileStructure(string YandexDiskDirectory = "")
        {
            using (var request = new HttpRequestMessage(new HttpMethod("Get"),
                $@"https://cloud-api.yandex.net/v1/disk/resources?path=/{YandexDiskBaseFolder}/{YandexDiskDirectory}&fields=_embedded.items.name,_embedded.items.type&limit=100"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);
                HttpResponseMessage Response = httpClient.SendAsync(request).Result;
                string JSONResult = Response.Content.ReadAsStringAsync().Result;
                var str = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>>(JSONResult)["_embedded"]["items"];
                return str.Select(dict => new YandexDiskFileModel() { Name = dict["name"], Type = dict["type"] }).ToList();
            }
        }

        public string DownloadFile(string YandexDiskPath, string ComputerPath, bool UseNewWebClient = false)
        {

            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("Get"),
                $@"https://cloud-api.yandex.net/v1/disk/resources/download?path=/{YandexDiskBaseFolder}/{YandexDiskPath}/"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);

                HttpResponseMessage Response = httpClient.SendAsync(request).Result;

                if (Response.ToString().Contains(YandexDiskServerMessages.FileNotFound)) { throw new FileNotFoundException(); }

                string DownloadURL = JsonConvert.DeserializeObject<Dictionary<string, object>>(Response.Content.ReadAsStringAsync().Result)["href"].ToString();

                if (File.Exists(ComputerPath)) { File.Delete(ComputerPath); }

                using (WebClient WC = new WebClient())
                { 
                    WC.DownloadFile(new Uri(DownloadURL), ComputerPath);
                }

                if (File.Exists(ComputerPath))
                { return "Success: File downloaded"; }
                else
                { return "Error: File not downloaded"; }
            }
        }

        public string UploadFile(string LocalFilePath, string YandexDiskFilePath = null)
        {
            YandexDiskFilePath = YandexDiskFilePath ?? LocalFilePath.Split('\\').Last();

            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("Get"),
                $@"https://cloud-api.yandex.net/v1/disk/resources/upload?path=/{YandexDiskBaseFolder}/{YandexDiskFilePath}/"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);

                HttpResponseMessage Response = httpClient.SendAsync(request).Result;

                var serverGetRefResponse = Response.Content.ReadAsStringAsync().Result;

                if (serverGetRefResponse.Contains(YandexDiskServerMessages.AlreadyExists))
                {
                    throw new FileAlreadyExistsException();
                }

                string UploadURL = JsonConvert.DeserializeObject<Dictionary<string, object>>(serverGetRefResponse)["href"].ToString();
                string result;
                using (var wc = new WebClient())
                {
                    result = String.Join(" ", wc.UploadFile(UploadURL, LocalFilePath));
                }
                return result;
            }
        }

        public string CreateFolder(string NewFolderPath)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("Put"),
                $@"https://cloud-api.yandex.net/v1/disk/resources/?path=/{YandexDiskBaseFolder}/{NewFolderPath}/"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);
                return httpClient.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }
        }

        //Case of the File path matters
        public string DeleteFile(string FileToDeletePath)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("DELETE"),
                    $@"https://cloud-api.yandex.net/v1/disk/resources?path=/{YandexDiskBaseFolder}/{FileToDeletePath}"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);
                string Response = httpClient.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
                return Response;
            }
        }

        public void DeleteFileAsync(string FileToDeletePath)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("DELETE"),
                $@"https://cloud-api.yandex.net/v1/disk/resources?path=/{YandexDiskBaseFolder}/{FileToDeletePath}"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "OAuth " + this.Token);
                string Response = httpClient.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }
        }

        public void UploadFileWithContent(string FileName, string FileContent = "")
        {
            string FilePath = this.ComputerBasePath + @"\" + FileName;

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            File.AppendAllText(FilePath, FileContent);
            this.UploadFile(FilePath);
            File.Delete(FilePath);
        }

        public string GetMessage(string YandexDiskMessagePath, bool DeleteAfterReading = true, bool UseNewWebClient = false)
        {
            string[] SplittedPath = YandexDiskMessagePath.Split('/');
            string FilePath = YandexDiskMessagePath.Replace("/", "");
            string result = "";

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            this.DownloadFile(YandexDiskMessagePath, FilePath, UseNewWebClient);

            try
            {
                result = File.ReadAllText(FilePath);

                if (DeleteAfterReading) { this.DeleteFile(YandexDiskMessagePath); }
                return result;
            }
            catch (Exception)
            {
                return "Error: message wasn't acquired";
            }

        }


        public string ReadFileFromYandexDisk(string YandexDiskPath, bool UseNewWebCLient = false)
        {
            string FileName = YandexDiskPath.Split('/')[YandexDiskPath.Split('/').GetUpperBound(0)];

            DownloadFile(YandexDiskPath, ComputerBasePath + @"\" + FileName, UseNewWebCLient);

            string result = File.ReadAllText(ComputerBasePath + @"\" + FileName);
            File.Delete(ComputerBasePath + @"\" + FileName);
            return result;
        }

        private static class YandexDiskServerMessages
        {
            public const string AlreadyExists = "already exists";
            public const string FileNotFound = "ReasonPhrase: 'NOT FOUND'";
        }
    }
}

