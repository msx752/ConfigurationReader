using AutoMapper;
using ConfigurationLib.Dashboard.Models;
using ConfigurationLib.Dashboard.Models.Dtos;
using ConfigurationLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Kernel.Bus;
using Shared.Kernel.Messages;
using Shared.Kernel.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConfigurationLib.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IExceptionLogRepository _exceptionLogRepository;
        private readonly IConfigurationReader _configurationReader;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public HomeController(IConfigurationReader configurationReader, IMessageBus messageBus, IConfiguration configuration, IExceptionLogRepository exceptionLogRepository, IMapper mapper)
        {
            _configurationReader = configurationReader;
            _messageBus = messageBus;
            _configuration = configuration;
            _exceptionLogRepository = exceptionLogRepository;
            _mapper = mapper;
        }

        [HttpGet("Error/{message}/{stackTrace}")]
        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error([FromRoute] string message = null, string stackTrace = null)
        {
            try
            {
                ExceptionLogEvent @event = new()
                {
                    ApplicationName = _configuration["AppName"],
                    CorrelationId = Guid.NewGuid(),
                    ExceptionMessage = message,
                    ExceptionStackTrace = stackTrace,
                };

                await _messageBus.PublishEvent(@event);
            }
            catch
            {
            }

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
                model.CurrentEnvironment.IsloggingEnabled = _configurationReader.GetValue<bool>("IsloggingEnabled");
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

        public async Task<IActionResult> GlobalExceptionLog()
        {
            try
            {
                HomeViewModel model = new HomeViewModel() { ExceptionLogs = new() };
                var response =  await _exceptionLogRepository.ListAsync();
                model.ExceptionLogs = _mapper.Map<List<ExceptionLogDto>>(response);

                return View(model);
            }
            catch (Exception e)
            {
                var routeValues = new { message = e.Message, stackTrace = e.StackTrace };
                return RedirectToAction("Error", "Home", routeValues);
            }
        }
    }
}