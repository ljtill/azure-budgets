using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Text.Json;

using Fluent = Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.Subscription;

namespace Microsoft.AppInnovation.Budgets
{
    public static class HttpTrigger1
    {
        private static AlertRequest alert;

        [Function("HttpTrigger1")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("HttpTrigger1");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            /*
                Parse payload
                Authenticate with fabric
                Update Subscription state
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
                // TODO: Authentication switch (Local, Cloud)
                // TODO: Switch to environment variables (local.settings.json)
                var credentials = Fluent.SdkContext.AzureCredentialsFactory.FromFile("./credentials.json");

                var client = new SubscriptionClient(credentials);
                //var subscriptions = client.Subscriptions.List();
                var subscription = client.Subscriptions.Get(alert.data.SubscriptionId);

                logger.LogInformation("Updating subscription state.");
                //client.Subscription.Cancel(subscription.SubscriptionId);
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
            //response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            //response.WriteString("Welcome to Azure Functions!");

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
