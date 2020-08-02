using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MVCClient.BackgroundHosts
{
    public class BasicStatsHelper : IStatsHelper
    {
        private string _uniqueSymbol;
        private BackgroundStatsQueue _processQueue;
        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private static string _folder = "BasicStats";
        private string _fullPath;
        private string _fullPathWithFileName;
        private StreamWriter _streamWriter;
        private DateTime _nextInterval;
        public BasicStatsHelper(string uniqueSymbol, BackgroundStatsQueue processQueue)
        {
            _uniqueSymbol = uniqueSymbol;
            _processQueue = processQueue;
            DateTime now = DateTime.Now;
            _nextInterval = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            _nextInterval = _nextInterval.AddHours(1);
            //_nextInterval = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            //_nextInterval = _nextInterval.AddMinutes(1);
            _fullPath = System.IO.Path.Combine(_path, _folder);

            if (!System.IO.Directory.Exists(_fullPath))
            {
                System.IO.Directory.CreateDirectory(_fullPath);
            }

            _fullPathWithFileName = Path.Combine(_fullPath, DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + "_" + _uniqueSymbol + ".txt");
            _streamWriter = new StreamWriter(_fullPathWithFileName);

        }
        public async Task PostMessageAsync(string message)
        {
            if(DateTime.Now > _nextInterval)
            {
                _nextInterval = _nextInterval.AddHours(1);
                //_nextInterval = _nextInterval.AddMinutes(2);
                CreateNewFile();
            }

            await _streamWriter.WriteLineAsync(message);
        }

        private void CreateNewFile()
        {
            _streamWriter.Close();
            _processQueue.QueueItem(_fullPathWithFileName);
            _streamWriter = null;
            _fullPathWithFileName = Path.Combine(_fullPath, DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + "_" + _uniqueSymbol + ".txt");
            _streamWriter = new StreamWriter(_fullPathWithFileName);
        }
    }
    public class StatsService : BackgroundService
    {
        private BackgroundStatsQueue _filesToProcess;
        private readonly ILogger<StatsService> _logger;

        public StatsService(BackgroundStatsQueue queue, ILogger<StatsService> logger)
        {
            _filesToProcess = queue;
            _logger = logger;

        }
        public IStatsHelper GetStatsHelper(string uniqueSymbol)
        {
            return new BasicStatsHelper(uniqueSymbol, _filesToProcess);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string file = await _filesToProcess.DequeueAsync(stoppingToken);

                    //process file
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
