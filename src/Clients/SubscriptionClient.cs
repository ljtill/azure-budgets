using Budgets.Models;

namespace Budgets.Clients;

public class SubscriptionClient
{
    private readonly ILogger _logger;
    private readonly string _accessToken;
    private string _apiVersion = "2020-01-01";

    public SubscriptionClient(ILogger logger)
    {
        _logger = logger;
        _accessToken = GetAccessToken();
    }

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
    private HttpResponseMessage NewApiRequest(string route)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync($"https://management.azure.com/{route}?api-version={_apiVersion}").Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: Improve exception messages
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new Exception($"Unable to authenticate.");
                    case HttpStatusCode.NotFound:
                        throw new Exception($"Unable to retrieve subscription.");
                    default:
                        throw new Exception($"Unable to process request (Status={response.StatusCode}.)");
                }
            }

            return response;
        }
    }
    private Subscription ParseApiRequest(HttpContent content)
    {
        var contentRaw = content.ReadAsStringAsync().Result;
        var contentParsed = JsonSerializer.Deserialize<Subscription>(contentRaw);
        return contentParsed;
    }
    private SubscriptionTags ParseTagsApiRequest(HttpContent content)
    {
        var contentRaw = content.ReadAsStringAsync().Result;
        var contentParsed = JsonSerializer.Deserialize<SubscriptionTags>(contentRaw);
        return contentParsed;
    }

    public void Get(string subscriptionId)
    {
        var response = NewApiRequest($"/subscriptions/{subscriptionId}");
        var responseContent = response.Content;
        var subscription = ParseApiRequest(responseContent);
    }
    public void GetTags(string subscriptionId)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync($"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/tags?api-version={_apiVersion}").Result;
            if (response.StatusCode != HttpStatusCode.OK)
            { }
        }
    }
}