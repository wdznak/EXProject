using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public class ConnectionsManager
    {
        private BackgroundWSQueue _queue;
        private CryptoDataScraperService _scraperService;
        public ConnectionsManager(BackgroundWSQueue queue, CryptoDataScraperService scraperService)
        {
            _queue = queue;
            _scraperService = scraperService;
        }

        public void AddConnection(BinanceConnection connection)
        {
            _queue.QueueBackgroundWS(connection);
        }

        public ICollection<string> GetConnections()
        {
            return _scraperService.GetActiveConnections();
        }
    }
}
