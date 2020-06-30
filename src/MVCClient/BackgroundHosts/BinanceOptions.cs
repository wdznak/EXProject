using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public class BinanceConnection
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string DepthAddress { get; set; }
        public string Symbol { get; set; }
    }

    public class BinanceOptions
    {
        public const string ExchangeName = "Binance";

        public List<BinanceConnection> Connections { get; set; }
    }
}
