using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Master.Resilience
{
    public class PoliciesFactory : IPoliciesFactory
    {
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly int _retryCount;
        private readonly int _exceptionsAllowedBeforeBreaking;
        private Policy[] _policies;

        public PoliciesFactory(ILogger<ResilientHttpClient> logger,
            int exceptionsAllowedBeforeBreaking = 3,
            int retryCount = 3)
        {
            _logger = logger;
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            _retryCount = retryCount;
            _policies = CreatePolicies();
        }

        public Policy[] Policies() => _policies;

        private Policy[] CreatePolicies()
            => new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    // number of retries
                    _retryCount,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.ExecutionKey}, " +
                            $"due to: {exception}.";
                        _logger.LogWarning(msg);
                        _logger.LogDebug(msg);
                    }),
                Policy.Handle<HttpRequestException>()
                .CircuitBreakerAsync( 
                   // number of exceptions before breaking circuit
                   _exceptionsAllowedBeforeBreaking,
                   // time circuit opened before retry
                   TimeSpan.FromMinutes(1),
                   (exception, duration) =>
                   {
                        // on circuit opened
                        _logger.LogTrace("Circuit breaker opened");
                   },
                   () =>
                   {
                        // on circuit closed
                        _logger.LogTrace("Circuit breaker reset");
                   })
            };
    }
}
