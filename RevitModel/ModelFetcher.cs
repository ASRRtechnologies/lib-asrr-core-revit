using System;
using System.Net.Http;

namespace ASRR.Revit.Core.RevitModel
{
    public class ModelFetcher
    {
        private readonly HttpService _httpService;

        public ModelFetcher(HttpService httpService)
        {
            _httpService = httpService ?? new HttpService();
        }

        public bool Fetch(string url, string destinationPath)
        {
            
            return false;

        }

        private bool DownloadModel(string modelName, string blobId)
        {
            return false;
        }
    }
}