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

namespace Microsoft.AppInnovation.Budgets
{
    public static class HttpTrigger1
    {
        private static AlertRequest alert;
        private static AzureCredentials credentials;

        [Function("HttpTrigger1")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HttpTrigger1");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            /*
                ? Write table storage logs
            */

            try
            {
                logger.LogInformation("Parsing HTTP request body data.");
                alert = ParseHttpRequest(req);
            }
            catch
            {
                // TODO: Custom error logging
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            try
            {
                logger.LogInformation("Authenticating with Azure endpoints.");
                credentials = GetCredentials(logger);

                logger.LogInformation("Updating Azure subscription state.");
                DisableSubscription(logger, alert);
            }
            catch (Exception e)
            {
                // TODO: Custom error logging
                logger.LogError($"Exception thrown during authentication process (Reason='{e.Message}')");
                // TODO: Return error response (subscription not found)
                // TODO: Return error response (insufficient permissions)
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static AlertRequest ParseHttpRequest(HttpRequestData req)
        {
            var body = new StreamReader(req.Body).ReadToEnd();
            if (string.IsNullOrEmpty(body))
            {
                // TODO: Implement custom exception
                throw new Exception();
            }

            return JsonSerializer.Deserialize<AlertRequest>(body);
        }

        private static AzureCredentials GetCredentials(ILogger logger)
        {
            if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
            {
                logger.LogDebug("Running in development mode.");
                var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
                var credentials = Fluent.SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId, Fluent.AzureEnvironment.AzureGlobalCloud);
            }
            else
            {
                logger.LogDebug("Running in production mode.");
                var credentials = Fluent.SdkContext.AzureCredentialsFactory.FromSystemAssignedManagedServiceIdentity(Fluent.Authentication.MSIResourceType.AppService, Fluent.AzureEnvironment.AzureGlobalCloud);
            }

            return credentials;
        }

        private static void DisableSubscription(ILogger logger, AlertRequest alert)
        {
            var client = new SubscriptionClient(credentials);
            logger.LogDebug("Validating subscription access.");
            var subscription = client.Subscriptions.Get(alert.data.SubscriptionId);

            logger.LogDebug("Setting subscription to disable state.");
            //client.Subscription.Cancel(subscription.SubscriptionId);
        }
    }

    public class AlertRequest
    {
        public AlertRequest() { }
        public string schemaId { get; set; }
        public AlertRequestData data { get; set; }
    }

    public class AlertRequestData
    {
        public AlertRequestData() { }
        public string SubscriptionName { get; set; }
        public string SubscriptionId { get; set; }
        public string SpendingAmount { get; set; }
        public string BudgetStartDate { get; set; }
        public string Budget { get; set; }
        public string Unit { get; set; }
        public string BudgetCreator { get; set; }
        public string BudgetName { get; set; }
        public string BudgetType { get; set; }
        public string ResourceGroup { get; set; }
        public string NotificationThresholdAmount { get; set; }
    }
}
