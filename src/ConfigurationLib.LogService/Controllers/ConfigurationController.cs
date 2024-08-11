using ConfigurationLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConfigurationLib.LogService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        IConfigurationReader _configurationReader;
        IConfiguration _configuration;
        public ConfigurationController(IConfigurationReader configurationReader, IConfiguration configuration)
        {
            _configurationReader = configurationReader;
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            object obj = new
            {
                CurrentApplicationName= _configuration["AppName"],
                RefreshInterval = _configuration["RefreshTimerIntervalInMs"],
                SiteName = _configurationReader.GetValue<string>("SiteName"),
                MaxItemCount = _configurationReader.GetValue<int>("MaxItemCount"),
                IsBasketEnabled = _configurationReader.GetValue<bool>("IsBasketEnabled"),
                IsloggingEnabled = _configurationReader.GetValue<bool>("IsloggingEnabled"),
            };

            return new JsonResult(obj, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
        }
    }
}
