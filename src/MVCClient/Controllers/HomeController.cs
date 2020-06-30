using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using MVCClient.Models;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Text;
using Microsoft.AspNetCore.Server.IIS;
using System.Text.Json;
using MVCClient.BackgroundHosts;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [Route("api")]
        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync("https://localhost:5003/identity");
            ViewBag.Json = JArray.Parse(content).ToString();

            return View("json");
        }

        public IActionResult Connections([FromServices] ConnectionsManager manager)
        {
            ICollection<string> openConnections = manager.GetConnections();
            return View(openConnections);
        }

        [Route("websocket")]
        public IActionResult ConnectSocket([FromServices] BackgroundWSQueue service)
        {
            BinanceConnection connection = new BinanceConnection()
            {
                Address = "wss://stream.binance.com:9443/stream?streams=btcusdt@depth/btcusdt@trade",
                Description = "USDT-BTC trades, depth",
                DepthAddress = "https://www.binance.com/api/v1/depth?symbol=BTCUSDT&limit=1000",
                Symbol = "USDT-BTC"
            };
            service.QueueBackgroundWS(connection);
            return View("Connections", connection);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
