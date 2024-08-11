using AutoMapper;
using ConfigurationLib.Dashboard.Models;
using ConfigurationLib.Models;
using ConfigurationLib.Models.Dtos;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Shared.Kernel.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConfigurationLib.Dashboard.Controllers
{
    public class ApplicationConfigurationController : Controller
    {
        private readonly IApplicationConfigurationRepository _applicationConfigurationRepository;
        private readonly IMapper _mapper;

        public ApplicationConfigurationController(IApplicationConfigurationRepository applicationConfigurationRepository, IMapper mapper)
        {
            _applicationConfigurationRepository = applicationConfigurationRepository;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            ApplicationConfigurationViewModel model = new();
            var response = await _applicationConfigurationRepository.ListAsync();
            model.Listing = _mapper.Map<List<ApplicationConfigurationDto>>(response);

            return View(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            if (!ObjectId.TryParse(id, out var idValue))
                throw new InvalidCastException(nameof(id));

            ApplicationConfigurationViewModel model = new();

            var response = await _applicationConfigurationRepository.GetById(idValue);
            model.Editing = _mapper.Map<ApplicationConfigurationDto>(response);

            return View(model);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, ApplicationConfigurationViewModel model)
        {
            try
            {
                if (id == null)
                    throw new ArgumentNullException(nameof(id));

                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (model.Editing == null)
                    throw new ArgumentNullException(nameof(model.Editing));

                var editedModel = _mapper.Map<ApplicationConfiguration>(model.Editing);

                await _applicationConfigurationRepository.Update(editedModel);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var routeValues = new { message = e.Message, stackTrace = e.StackTrace };
                return RedirectToAction("Error", "Home", routeValues);
            }
        }

        [HttpGet()]

        public async Task<IActionResult> Create()
        {
            ApplicationConfigurationViewModel model = new();

            model.Creating = new();

            return View(model);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(ApplicationConfigurationViewModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (model.Creating == null)
                    throw new ArgumentNullException(nameof(model.Creating));

                var createdModel = _mapper.Map<ApplicationConfiguration>(model.Creating);

                await _applicationConfigurationRepository.Add(createdModel);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var routeValues = new { message = e.Message, stackTrace = e.StackTrace };
                return RedirectToAction("Error", "Home", routeValues);
            }
        }
    }
}
