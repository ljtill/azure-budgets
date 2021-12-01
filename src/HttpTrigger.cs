using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Text.Json;

using Microsoft.AppInnovation.Budgets.Clients;
using Microsoft.AppInnovation.Budgets.Exceptions;
using Microsoft.AppInnovation.Budgets.Schemas;

namespace Microsoft.AppInnovation.Budgets
{
    public class HttpTrigger
    {
        private readonly ILogger _logger;

        public HttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
        }

        [Function("HttpTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                // Parse
                _logger.LogInformation("Parsing function request.");
                var subscriptionId = ParseHttpRequest(req).Data.SubscriptionId;

                // Client
                var subscriptionClient = new SubscriptionClient(_logger);

                // Check
                _logger.LogInformation("Checking subscription.");
                if (CheckSubscriptionTags(subscriptionClient, subscriptionId) == false)
                {
                    // Disable
                    _logger.LogInformation("Disabling subscription.");
                    DisableSubscription(subscriptionClient, subscriptionId);
                }
                else
                {
                    // Exclude
                    _logger.LogInformation("Excluded subscription detected.");
                }
            }
            catch (ParserException pe)
            {
                _logger.LogError(pe.Message);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }

        private Alert ParseHttpRequest(HttpRequestData req)
        {
            var body = new StreamReader(req.Body).ReadToEnd();
            if (string.IsNullOrEmpty(body))
            {
                throw new ParserException("Unknown request schema provided.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Alert>(body, options);
        }

        private bool CheckSubscriptionTags(SubscriptionClient client, string SubscriptionId)
        {
            _logger.LogDebug("Retrieving subscription.");

            // TODOD: Implementation

            return false;
        }

        private void DisableSubscription(SubscriptionClient client, string SubscriptionId)
        {
            _logger.LogDebug("Disabling subscription.");

            // TODO: Implementation

            return;
        }
    }
}
