using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ASRR.Revit.Core.Elements;

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
            var task = Task.Run(async () => await _httpClient.GetAsync(Utilities.CleanUpPath(path)));
            task.Wait();

            if (task.IsCompleted) return task.Result;

            _logger.Error($"Failed to get {path}");
            return null;
        }

        public HttpResponseMessage Post(string path, HttpContent content)
        {
            var task = Task.Run(async () => await _httpClient.PostAsync(Utilities.CleanUpPath(path), content));
            task.Wait();

            if (task.IsCompleted) return task.Result;
            
            _logger.Error($"Failed to post to {path}");
            return null;
        }
    }
}