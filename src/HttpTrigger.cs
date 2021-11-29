using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Text.Json;

using Azure.Core;
using Azure.Identity;

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

            HttpResponseData response;

            try
            {
                _logger.LogInformation("Parsing HTTP request body data.");
                alert = ParseHttpRequest(req);
            }
            catch
            {
                // TODO: Custom error logging
                // TODO: JSON Response message
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            try
            {
                _logger.LogInformation("Authenticating with identity endpoints.");
                var accessToken = GetAccessToken(_logger);
                _logger.LogInformation($"Token: {accessToken}");

                _logger.LogInformation("Checking Azure subscription exclusion tags.");
                CheckSubscriptionTags(_logger, alert.Data.SubscriptionId);

                _logger.LogInformation("Updating Azure subscription state.");
                DisableSubscription(_logger, alert.Data.SubscriptionId);

                response = req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                // TODO: Custom error logging
                // TODO: JSON Response message
                _logger.LogError($"Exception thrown during authentication process (Reason='{e.Message}')");
                // TODO: Return error response (subscription not found)
                // TODO: Return error response (insufficient permissions)
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

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

        private string GetAccessToken(ILogger logger)
        {
            AccessToken token;

            if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
            {
                logger.LogDebug("Running in development mode.");
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");

                logger.LogDebug("Authenticating client secret credential.");
                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

                logger.LogDebug("Retrieiving access token.");
                token = credential.GetToken(new TokenRequestContext());
            }
            else
            {
                logger.LogDebug("Running in production mode.");
                var clientId = Environment.GetEnvironmentVariable("AZURE_MANAGED_IDENTITY");

                logger.LogDebug("Authenticating managed identity credential.");
                var credential = new ManagedIdentityCredential(clientId);

                logger.LogDebug("Retrieving access token.");
                token = credential.GetToken(new TokenRequestContext());
            }

            return token.Token;
        }

        private void CheckSubscriptionTags(ILogger logger, string SubscriptionId)
        {
            logger.LogDebug("Getting subscription tags.");
            // TODO: Add HTTP call
        }

        private void DisableSubscription(ILogger logger, string SubscriptionId)
        {
            logger.LogDebug("Setting subscription to disable state.");
            // TODO: Add HTTP call
        }
    }
}
