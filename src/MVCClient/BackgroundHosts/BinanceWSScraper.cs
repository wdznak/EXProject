using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Net.WebSockets;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.SignalR;

namespace MVCClient.BackgroundHosts
{
    public class BinanceWSScraper
    {
        public BinanceConnection connectionDetails { get; private set; }
        private static short _MAXRECONNECTION = 5;
        private short _reconnections = 0;
        private ILogger<BinanceWSScraper> _logger;
        private IStorage _storage;

        public BinanceWSScraper(ILogger<BinanceWSScraper> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void Init(BinanceConnection connectionDetails)
        {
            this.connectionDetails = connectionDetails;
            _storage.Init(this.connectionDetails.Symbol);
        }

        public async Task ConnectAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await OpenSocketAsync(stoppingToken);
                }
                catch (Exception ex) when (!(ex is TaskCanceledException))
                {
                    _reconnections++;
                    if(_reconnections >= _MAXRECONNECTION)
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

                Console.WriteLine("Connecting to the websocket...");
                await webSocket.ConnectAsync(new Uri(connectionDetails.Address), stoppingToken);

                var buffer = new byte[4 * 1024];
                var segment = new ArraySegment<byte>(buffer);
                WebSocketReceiveResult result;

                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(segment, stoppingToken);
                    string msg = Encoding.UTF8.GetString(segment.Array, 0, result.Count);
                    await _storage.WriteAsync(msg);
                }   
            }
        }
    }
}
