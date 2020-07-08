using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace MVCClient.BackgroundHosts
{
    class WSScraperData
    {
        public CancellationTokenSource tokenSource;
        public BinanceWSScraper wsScraper;
        public Task wsScraperTask;

        public WSScraperData()
        {
            tokenSource = new CancellationTokenSource();
        }
    }
    public class CryptoDataScraperService : BackgroundService
    {
        public BackgroundWSQueue _connectionsQueue;
        private readonly ILogger<CryptoDataScraperService> _logger;
        private readonly IConfiguration _configuration;
        private BinanceOptions _binanceOptions { get; set; }
        private ConcurrentDictionary<String, WSScraperData> _wsConnections = new ConcurrentDictionary<string, WSScraperData>();
        private IServiceScopeFactory _scopeFactory;

        public CryptoDataScraperService(BackgroundWSQueue connectionsQueue, ILogger<CryptoDataScraperService> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _connectionsQueue = connectionsQueue;
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _binanceOptions = _configuration.GetSection(BinanceOptions.ExchangeName).Get<BinanceOptions>();
            _binanceOptions.Connections.ForEach(connection =>
            {
                //_connectionsQueue.QueueBackgroundWS(connection);
            });

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {

                while (!stoppingToken.IsCancellationRequested)
                {
                    BinanceConnection wsConnection = null;
                    try
                    {
                        wsConnection = await _connectionsQueue.DequeueAsync(stoppingToken);

                        WSScraperData data = new WSScraperData()
                        {
                            wsScraper = scope.ServiceProvider.GetRequiredService<BinanceWSScraper>()
                        };

                        data.wsScraper.Init(wsConnection);

                        _logger.LogInformation("Connecting to {0}.", wsConnection.Symbol);

                        data.wsScraperTask = Task.Run(async () =>
                        {
                            await data.wsScraper.ConnectAsync(data.tokenSource.Token)
                                .ContinueWith((antecedent, wsConnection) =>
                                {
                                    var binConnection = wsConnection as BinanceConnection;

                                    _wsConnections.TryRemove(binConnection.Symbol, out var data);

                                    foreach (var ex in antecedent.Exception.InnerExceptions)
                                    {
                                        if (ex is TaskCanceledException)
                                        {
                                            _logger.LogInformation("WebSocket connetion to {0} was closed.", binConnection.Symbol);
                                        }
                                        else
                                        {
                                            _logger.LogError("WebSocket connetion to {0} has failed. Exception: {1}.", binConnection.Symbol, ex);
                                        }
                                    }

                                }, wsConnection, TaskContinuationOptions.OnlyOnFaulted);
                        });


                        _wsConnections.TryAdd(wsConnection.Symbol, data);
                        _logger.LogInformation("Active connections count = {0}", _wsConnections.Count);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        if (wsConnection != null)
                        {
                            _logger.LogError(ex, "Error occured connecting: {Address}", wsConnection.Address);
                        }
                        else
                        {
                            _logger.LogError(ex, "Websocket connection dequeuing error.");
                        }
                    }
                }

                _logger.LogInformation("BackgroundProcessing is finished.");

                ShutDown();
            }
        }

        public ICollection<string> GetActiveConnections()
        {
            return _wsConnections.Keys;
        }

        private void ShutDown()
        {
            List<Task> taskList = new List<Task>();
            foreach (var webSocket in _wsConnections)
            {
                taskList.Add(webSocket.Value.wsScraperTask);
                webSocket.Value.tokenSource.Cancel();
            }

            _logger.LogInformation("Awaiting websockets to be closed.");

            Task.WaitAll(taskList.ToArray());

            _logger.LogInformation("Websockets closed.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            
            await base.StopAsync(stoppingToken);
        }
    }
}
