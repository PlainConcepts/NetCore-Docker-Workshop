using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Master.Resilience
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when invoking HTTP services. 
    /// Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ResilientHttpClient> _logger;
        private ConcurrentDictionary<string, PolicyWrap> _policyWrappers;
        private readonly IHttpContextAccessor _httpContextAccessor;
        IPoliciesFactory _policiesFactory;

        public ResilientHttpClient(IPoliciesFactory policiesFactory, ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _logger = logger;
            _policiesFactory = policiesFactory;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
            _httpContextAccessor = httpContextAccessor;
        }


        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            return DoPostPutAsync(HttpMethod.Post, uri, item);
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item)
        {
            return DoPostPutAsync(HttpMethod.Put, uri, item);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);               

                return await _client.SendAsync(requestMessage);
            });
        }


        public Task<string> GetStringAsync(string uri)
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                var response = await _client.SendAsync(requestMessage);

                // raise exception if HttpResponseCode 500 
                // needed for circuit breaker to track fails

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return await response.Content.ReadAsStringAsync();
            });
        }

        private Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item)
        {

            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(requestMessage);

                // raise exception if HttpResponseCode 500 
                // needed for circuit breaker to track fails

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policiesFactory.Policies());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all 
            // the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }


        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }
    }
}
