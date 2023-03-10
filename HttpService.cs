using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ASRR.Revit.Core.Elements;
using System.Linq;

namespace ASRR.Revit.Core
{
    public class HttpService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient _httpClient;

        public HttpService()
        {
            _httpClient = new HttpClient();
        }

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public void SetBaseAddress(string baseUri)
        {
            _httpClient.BaseAddress = new Uri(baseUri);
        }

        public HttpResponseMessage Get(string path)
        {
            var task = Task.Run(async () => await _httpClient.GetAsync(CleanUpPath(path)));
            task.Wait();

            if (task.IsCompleted) return task.Result;

            _logger.Error($"Failed to get {path}");
            return null;
        }


        public byte[] Download(string path)
        {
            var task = Task.Run(async () => await _httpClient.GetByteArrayAsync(CleanUpPath(path)));
            task.Wait();

            if (task.IsCompleted) return task.Result;

            _logger.Error($"Failed to download from {path}");
            return null;
        }

        public HttpResponseMessage Post(string path, HttpContent content)
        {
            var task = Task.Run(async () => await _httpClient.PostAsync(CleanUpPath(path), content));
            task.Wait();

            if (task.IsCompleted) return task.Result;
            
            _logger.Error($"Failed to post to {path}");
            return null;
        }

        private static string CombineUris(params string[] uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            var urisList = uris.ToList();

            var result = "";

            for (var i = 0; i < urisList.Count; i++)
                if (urisList[i] != null)
                {
                    var trimmedUri = urisList[i];
                    trimmedUri = trimmedUri.TrimStart('/', '\\');
                    trimmedUri = trimmedUri.TrimEnd('/', '\\');
                    var slash = i == 0 ? "" : "/";
                    result += $"{slash}{trimmedUri}";
                }

            return result;
        }

        private static string CleanUpPath(string path)
        {
            return path == null ? throw new ArgumentNullException(nameof(path)) : CombineUris(path);
        }
    }
}