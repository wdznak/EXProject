using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public class BinanceWSScraper
    {
        public BinanceConnection connectionDetails { get; private set; }
        private static short _MAXRECONNECTION = 5;
        private short _reconnections = 0;
        private ILogger<BinanceWSScraper> _logger;
        private IStorage _storage;
        private IStatsHelper _stats;

        public BinanceWSScraper(ILogger<BinanceWSScraper> logger)
        {
            _logger = logger;
        }

        public void Init(BinanceConnection connectionDetails)
        {
            this.connectionDetails = connectionDetails;
        }

        public async Task ConnectAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await OpenSocketAsync(stoppingToken);
                }
                catch (Exception ex) when (!(ex is TaskCanceledException))
                {
                    _reconnections++;
                    if (_reconnections >= _MAXRECONNECTION)
                    {
                        throw;
                    }
                }
            }
        }

        private async Task OpenSocketAsync(CancellationToken stoppingToken)
        {
            using (var webSocket = new ClientWebSocket())
            {
                webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(5);

                await webSocket.ConnectAsync(new Uri(connectionDetails.Address), stoppingToken);

                var buffer = new byte[4 * 1024];
                var segment = new ArraySegment<byte>(buffer);
                WebSocketReceiveResult result;

                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(segment, stoppingToken);
                    string msg = Encoding.UTF8.GetString(segment.Array, 0, result.Count);
                    if(_storage != null)  await _storage.WriteAsync(msg);
                    await _stats.PostMessageAsync(msg);
                }
            }
        }

        public IStorage Storage
        {
            set { if(_storage == null) _storage = value; }
        }

        public IStatsHelper Stats
        {
            set { if (_stats == null) _stats = value; }
        }
    }
}
