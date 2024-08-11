using ConfigurationLib.Dashboard.Models;
using ConfigurationLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ExceptionMessage = message, ExceptionStackTrace = stackTrace });
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CurrentEnvironment()
        {
            try
            {
                HomeViewModel model = new HomeViewModel() { CurrentEnvironment = new() };
                model.CurrentEnvironment.SiteName = _configurationReader.GetValue<string>("SiteName");
                model.CurrentEnvironment.MaxItemCount = _configurationReader.GetValue<int>("MaxItemCount");
                model.CurrentEnvironment.IsBasketEnabled = _configurationReader.GetValue<bool>("IsBasketEnabled");
                return View(model);
            }
            catch (Exception e)
            {
                var routeValues = new { message = e.Message, stackTrace = e.StackTrace };
                return RedirectToAction("Error", "Home", routeValues);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}