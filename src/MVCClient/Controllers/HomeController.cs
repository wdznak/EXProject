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

        public IActionResult Connections([FromServices] ConnectionsManager manager)
        {
            ConnectionsModel model = new ConnectionsModel();
            ICollection<string> openConnections = manager.GetConnections();

            foreach (var connection in openConnections)
            {
                model.activeConnections.Add(new BinanceConnectionModel { Symbol = connection });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AddConnection()
        {
            return View();
        }

        public IActionResult AddConnection([FromServices] BackgroundWSQueue service, BinanceConnectionModel model)
        {
            BinanceConnection connection = new BinanceConnection()
            {
                Address = model.Address,
                Description = model.Description,
                DepthAddress = model.DepthAddress,
                Symbol = model.Symbol
            };

            service.QueueBackgroundWS(connection);

            return RedirectToAction("Connections");
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
