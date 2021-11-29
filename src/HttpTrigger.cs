using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Text.Json;

using Fluent = Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.Subscription;

using Microsoft.AppInnovation.Budgets.Exceptions;
using Microsoft.AppInnovation.Budgets.Schema;

namespace Microsoft.AppInnovation.Budgets
{
    public class HttpTrigger
    {
        private readonly ILogger _logger;

        public HttpTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
        }

        private BudgetAlert alert;

        [Function("HttpTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                _logger.LogInformation("Parsing HTTP request body data.");
                alert = ParseHttpRequest(req);
            }
            catch
            {
                // TODO: Custom error logging
                // TODO: JSON Response message
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            try
            {
                _logger.LogInformation("Checking Azure subscription exclusion tags.");
                CheckSubscriptionTags(_logger, alert.Data.SubscriptionId);

                _logger.LogInformation("Updating Azure subscription state.");
                //DisableSubscription(_logger, credentials, alert);
            }
            catch (Exception e)
            {
                // TODO: Custom error logging
                // TODO: JSON Response message
                _logger.LogError($"Exception thrown during authentication process (Reason='{e.Message}')");
                // TODO: Return error response (subscription not found)
                // TODO: Return error response (insufficient permissions)
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private BudgetAlert ParseHttpRequest(HttpRequestData req)
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

            return JsonSerializer.Deserialize<BudgetAlert>(body, options);
        }

        private AzureCredentials GetCredentials(ILogger logger)
        {
            AzureCredentials credentials = null;

            if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
            {
                logger.LogDebug("Running in development mode.");
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
                credentials = Fluent.SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId, Fluent.AzureEnvironment.AzureGlobalCloud);
            }
            else
            {
                logger.LogDebug("Running in production mode.");
                credentials = Fluent.SdkContext.AzureCredentialsFactory.FromSystemAssignedManagedServiceIdentity(Fluent.Authentication.MSIResourceType.AppService, Fluent.AzureEnvironment.AzureGlobalCloud);
            }

            return credentials;
        }

        private SubscriptionClient GetSubscriptionClient(ILogger logger)
        {
            logger.LogDebug("Authenticating with Azure endpoints.");
            var credentials = GetCredentials(logger);

            logger.LogDebug("Creating Azure subscription client.");
            var client = new SubscriptionClient(credentials);

            return client;
        }

        private void CheckSubscriptionTags(ILogger logger, string SubscriptionId)
        {
            var client = GetSubscriptionClient(logger);

            logger.LogDebug("Getting subscription tags.");
            var subscription = client.Subscriptions.Get(SubscriptionId);

        }

        private void DisableSubscription(ILogger logger, AzureCredentials credentials, BudgetAlert alert)
        {
            var client = new SubscriptionClient(credentials);
            logger.LogDebug("Validating subscription access.");
            var subscription = client.Subscriptions.Get(alert.Data.SubscriptionId);

            logger.LogDebug("Setting subscription to disable state.");
            //client.Subscription.Cancel(subscription.SubscriptionId);
        }
    }
}
