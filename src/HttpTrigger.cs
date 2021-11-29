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

        [Function("HttpTrigger")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            BudgetAlert alert;

            #region Parse
            try
            {
                _logger.LogInformation("Parsing HTTP request body data.");
                alert = ParseHttpRequest(req);
            }
            catch
            {
                // TODO: Log message (parsing failed)
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            #endregion

            string accessToken;

            #region Authenticate
            try
            {
                _logger.LogInformation("Authenticating with identity endpoints.");
                accessToken = GetAccessToken();
            }
            catch (Exception e)
            {
                // TODO: Log message (invalid credentials)
                _logger.LogError($"Exception thrown during authentication process (Reason='{e.Message}')");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            #endregion

            #region Process
            try
            {
                _logger.LogInformation("Checking Azure subscription exclusion tags.");
                CheckSubscriptionTags(accessToken, alert.Data.SubscriptionId);

                _logger.LogInformation("Updating Azure subscription state.");
                DisableSubscription(accessToken, alert.Data.SubscriptionId);
            }
            catch (Exception e)
            {
                // TODO: Log message (subscription not found)
                // TODO: Log message (insufficient permissions)
                _logger.LogError($"Exception thrown during subscription process (Reason='{e.Message}')");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            #endregion

            return req.CreateResponse(HttpStatusCode.OK);
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

        private string GetAccessToken()
        {
            AccessToken token;
            var scopes = new string[] { "https://management.azure.com/.default" };

            if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
            {
                // TODO: Validate local.settings.json values are not null
                _logger.LogDebug("Running in development mode.");
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");

                _logger.LogDebug("Authenticating client secret credential.");
                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

                _logger.LogDebug("Retrieiving access token.");
                token = credential.GetToken(new TokenRequestContext(scopes));
            }
            else
            {
                _logger.LogDebug("Running in production mode.");
                var clientId = Environment.GetEnvironmentVariable("AZURE_MANAGED_IDENTITY");

                _logger.LogDebug("Authenticating managed identity credential.");
                var credential = new ManagedIdentityCredential(clientId);

                _logger.LogDebug("Retrieving access token.");
                token = credential.GetToken(new TokenRequestContext(scopes));
            }

            return token.Token;
        }

        private void CheckSubscriptionTags(string accessToken, string SubscriptionId)
        {
            _logger.LogDebug("Getting subscription tags.");
            // TODO: Add HTTP call
        }

        private void DisableSubscription(string accessToken, string SubscriptionId)
        {
            // TODO: Check Subscription state
            // TODO: Add HTTP call

            _logger.LogDebug("Setting subscription to disable state.");
            // TODO: Add HTTP call
        }
    }
}
