using NLog;
using System;
using System.IO;
using System.Net.Http;

namespace ASRR.Revit.Core.RevitModel
{
    public class ModelFetcher
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly HttpService _httpService;

        public ModelFetcher(HttpService httpService)
        {
            _httpService = httpService ?? new HttpService();
        }

        public bool Fetch(string url, string destinationPath)
        {
            if (url == null || destinationPath == null)
            {
                _logger.Error($"Fetching model failed. Input is null: url = {url}, destinationPath = {destinationPath}");
                return false;
            }

            if (!InitDestinationPath(destinationPath))
            {
                _logger.Error($"Fetching model failed. Invalid destination path: {destinationPath}");
                return false;
            }
            
            var fileBytes = _httpService.Download(url);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                _logger.Error($"Fetching model failed. Downloaded file is empty: url = {url}, destinationPath = {destinationPath}");
                return false;
            }

            File.WriteAllBytes(destinationPath, fileBytes);
            _logger.Info($"Downloaded model from {url} to {destinationPath}");
            return true;
        }

        private bool InitDestinationPath(string destinationPath)
        {
            try
            {
                var fileName = Path.GetFileName(destinationPath);
                if (string.IsNullOrEmpty(fileName))
                {
                    _logger.Error($"Fetching model failed. Destination path does not have a filename: {destinationPath}");
                    return false;
                }
                
                var directory = Path.GetDirectoryName(destinationPath);
                if (string.IsNullOrEmpty(directory)) return false;

                Directory.CreateDirectory(directory);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}