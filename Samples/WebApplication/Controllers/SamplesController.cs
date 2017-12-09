﻿using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DynaCore;
using WebApplication.Domain;

namespace WebApplication.Controllers
{
    [Route("samples")]
    public class SamplesController : DynaCore.Web.Controllers.DynaCoreController
    {
        private readonly ILogger<SamplesController> _logger;

        public SamplesController(ILogger<SamplesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(List<Sample>), (int)HttpStatusCode.OK)]
        public IActionResult QuerySamples()
        {
            _logger.LogDebug("QuerySamples called.");
            List<Sample> samples = new List<Sample>();
            samples.Add(new Sample()
            {
                Id = Guid.NewGuid(),
                Name = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                CreatedOn = DynaCoreApp.Instance.DateTimeProvider.Now
            });
            return Ok(samples);
        }
    }
}