using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MVCClient.BackgroundHosts
{
    public class FileStorage : IStorage
    {
        private static readonly int _MAXFILESIZE = 32000000;
        private readonly ILogger<FileStorage> _logger;
        private string _path;
        private string _fullPath;
        private string _folder;
        private string _symbol;
        private StreamWriter _streamWriter;
        public FileStorage(ILogger<FileStorage> logger, IConfiguration configuration)
        {
            _logger = logger;
            _path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            _folder = configuration["CryptoFolder"];
        }
        public async Task WriteAsync(string message)
        {
            if (_streamWriter.BaseStream.Length > _MAXFILESIZE)
            {
                createNewFile();
            }

            await _streamWriter.WriteLineAsync(message);
        }

        public void Init(string symbol)
        {
            _symbol = symbol;
            _fullPath = System.IO.Path.Combine(_path, _folder, _symbol);
            if (!System.IO.Directory.Exists(_fullPath))
            {
                var directory = System.IO.Directory.CreateDirectory(_fullPath);
                _logger.LogInformation("File path {0} created", directory.ToString());
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            _streamWriter = new StreamWriter(Path.Combine(_fullPath, date + "_" + _symbol + "_INIT.txt"));
        }

        private void createNewFile()
        {
            _streamWriter.Close();
            _streamWriter = null;
            string date = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            _streamWriter = new StreamWriter(Path.Combine(_fullPath, date + "_" + _symbol + ".txt"));
        }

        ~FileStorage()
        {
            _streamWriter?.Close();
        }
    }
}
