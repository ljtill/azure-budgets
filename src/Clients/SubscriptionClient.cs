using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

using System.Text.Json;

using Azure.Core;
using Azure.Identity;

using Microsoft.AppInnovation.Budgets.Schemas;

namespace Microsoft.AppInnovation.Budgets.Clients
{
    public class SubscriptionClient
    {
        private readonly ILogger _logger;
        private readonly string _accessToken;
        private string _apiVersion = "2020-01-01";

        #region Constructors
        public SubscriptionClient(ILogger logger)
        {
            _logger = logger;
            _accessToken = GetAccessToken();
        }
        #endregion

        #region Private Methods
        private string GetAccessToken()
        {
            _logger.LogInformation("Authenticating with identity endpoints.");

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

        private Subscription ParseApiRequest(HttpContent content)
        {
            var contentRaw = content.ReadAsStringAsync().Result;
            var contentParsed = JsonSerializer.Deserialize<Subscription>(contentRaw);

            return contentParsed;
        }
        #endregion

        #region Public Methods
        public void Get(string subscriptionId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync($"https://management.azure.com/subscriptions/{subscriptionId}?api-version={_apiVersion}").Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO: Improve exception messages
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new Exception($"Unable to authenticate.");
                        case HttpStatusCode.NotFound:
                            throw new Exception($"Unable to retrieve subscription (Id={subscriptionId}).");
                        default:
                            throw new Exception($"Request failed with status code {response.StatusCode}.");
                    }
                }

                var responseContent = response.Content;
                var subscription = ParseApiRequest(responseContent);
            }
        }
        #endregion
    }
}