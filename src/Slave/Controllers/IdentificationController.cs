using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Slave.Services;

namespace Slave.Controllers
{
    [Route("api/[controller]")]
    public class IdentificationController : Controller
    {
        private readonly Guid _id;

        public IdentificationController(GuidProvider provider)
        {
            _id = provider.Id;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok($"I am poor slave called {_id} living on {Environment.MachineName}");
        }

    }
}
