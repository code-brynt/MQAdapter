using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQSubscriberTwo.Models;
using System.Diagnostics;
using System.Linq;

namespace MQSubscriberTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ApplicationDbContext dbContext, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

        }

        public IActionResult Index()
        {
            var messages = _dbContext.Messages.ToList();
            return View(messages);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
