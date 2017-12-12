using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Master.Settings;
using Microsoft.Extensions.Options;
using Master.Services;
using System.Threading;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Master.Controllers
{
    [Route("api/[controller]")]
    public class GreetingsController : Controller
    {
        private readonly MySettings _cfg;
        private readonly Guid _guid;

        public GreetingsController(IOptions<MySettings> cfg,
         GuidProvider guidprovider)
        {
            _cfg = cfg.Value;
            _guid = guidprovider.Id;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var slaveMessage = await GetSlaveResponse();

            return Ok(new
            {
                Message = $"Hello Lord! I am your Master {_guid} running on {Environment.MachineName}",
                SlaveMessage = slaveMessage
            });
        }

        private async Task<string> GetSlaveResponse()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_cfg.SlaveUri}/api/identification");
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }

                    return $"Slave in {_cfg.SlaveUri} refuses to identify with {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error {ex.GetType().Name} ('{ex.Message}') in slave that is in {_cfg.SlaveUri}";
            }
        }
    }
}
