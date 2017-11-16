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
using Polly;
using Microsoft.Extensions.Logging;
using System.Net;
using Master.Resilience;

namespace Master.Controllers
{
    [Route("api/[controller]")]
    public class GreetingsController : Controller
    {
        private readonly MySettings _cfg;
        private readonly Guid _guid;
        private readonly ILogger<GreetingsController> _logger;
        private readonly IHttpClient _apiClient;
        private readonly IPoliciesFactory _policiesFactory;

        public GreetingsController(IOptions<MySettings> cfg,
         GuidProvider guidprovider,
         IHttpClient httpClient,
         IPoliciesFactory policiesFactory,
         ILogger<GreetingsController> logger)
        {
            _cfg = cfg.Value;
            _guid = guidprovider.Id;
            _logger = logger;
            _apiClient = httpClient;
            _policiesFactory = policiesFactory;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var slaveMessage = await GetSlaveResponseSimpleResilienceCase();

            //var slaveMessage = await GetSlaveResponseWithMyResilientHttpClient();

            // Triggers the azure function by posting an 
            // http call once the Slave has answered
            //await TriggerAzureFunction(_guid.ToString());

            return Ok(new {
                Message = $"Hello Lord! I am your Master {_guid} running on {Environment.MachineName}",
                SlaveMessage = slaveMessage
            });
        }

        private async Task<string> GetSlaveResponseWithMyResilientHttpClient()
        {
            return await _apiClient.GetStringAsync($"{_cfg.SlaveUri}/api/identification");
        }

        private async Task<string> GetSlaveResponseSimpleResilienceCase()
        {
            using (var client = new HttpClient())
            {
                return await HttpInvoker(async () =>
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_cfg.SlaveUri}/api/identification");

                    var response = await client.SendAsync(requestMessage);

                    // raise exception if HttpResponseCode 500 
                    // needed for circuit breaker to track fails

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        throw new HttpRequestException();
                    }

                    return await response.Content.ReadAsStringAsync();
                });
            }
        }

        private async Task<T> HttpInvoker<T>(Func<Task<T>> action)
        {
            var masterPolicies = _policiesFactory.Policies();
            var policyWrap = Policy.WrapAsync(masterPolicies);

            // Executes the action applying all 
            // the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action);
        }      

        private async Task TriggerAzureFunction(string slaveName)
        {
            var payload = JsonConvert.SerializeObject(new { name = slaveName });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(_cfg.AFUri, content);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
