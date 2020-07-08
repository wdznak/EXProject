using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.Models
{
    public class ConnectionsModel
    {
        public List<BinanceConnectionModel> activeConnections = new List<BinanceConnectionModel>();
        public List<BinanceConnectionModel> pendingConnections = new List<BinanceConnectionModel>();
        public List<BinanceConnectionModel> closedConnections = new List<BinanceConnectionModel>();
        public List<BinanceConnectionModel> failedConnections = new List<BinanceConnectionModel>();
    }
}
