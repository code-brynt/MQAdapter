using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQAdapter;
using MQPublisher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MQPublisher.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMQProxyService _mqProxyService;
        public HomeController(ILogger<HomeController> logger, IMQProxyService mqProxyService)
        {
            _logger = logger;
            _mqProxyService = mqProxyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Publish(string message)
        {
            //_mqProxyService.SendMessageToQueue("DEV.QUEUE.1", message);
            _mqProxyService.SendMessageToTopic("BroadCast", message);
            return View(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
