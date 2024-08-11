using ConfigurationLib.Dashboard.Models;
using ConfigurationLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ConfigurationLib.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigurationReader _configurationReader;
        public HomeController(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        [HttpGet("Error/{message}/{stackTrace}")]
        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error([FromRoute] string message = null, string stackTrace = null)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ExceptionMessage = message, ExceptionStackTrace= stackTrace });
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index1()
        {
            HomeViewModel model = new HomeViewModel() { CurrentEnvironment = new() };
            model.CurrentEnvironment.SiteName = _configurationReader.GetValue<string>("SiteName");
            model.CurrentEnvironment.MaxItemCount = _configurationReader.GetValue<int>("MaxItemCount");
            model.CurrentEnvironment.IsBasketEnabled = _configurationReader.GetValue<bool>("IsBasketEnabled");
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}