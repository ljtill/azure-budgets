using System.Threading;
using Azure;
using Budgets.Exceptions;
using Budgets.Models;

namespace Budgets;

public class HttpTrigger
{
    private readonly ILogger _logger;

    public HttpTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpTrigger>();
    }

    [Function("HttpTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var credential = new DefaultAzureCredential();
        
        try
        {
            var resourceId = GetResourceId(_logger, req);
            if (ExclusionTagExists(_logger, credential, resourceId) is false)
            {
                await DisableSubscription(_logger, credential, resourceId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("[HttpTrigger] {Message}", ex.Message);
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }

        return req.CreateResponse(HttpStatusCode.OK);
    }

    /// <summary>
    /// Parse Payload
    /// </summary>
    private static ResourceIdentifier GetResourceId(ILogger logger, HttpRequestData req)
    {
        logger.LogDebug("[HttpTrigger] Parsing notification request");
        
        var body = new StreamReader(req.Body).ReadToEnd();
        if (string.IsNullOrEmpty(body))
        {
            throw new ParserException("Unknown request schema provided.");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var notification = JsonSerializer.Deserialize<Notification>(body, options);
        
        if (notification is null)
        {
            throw new Exception("Notification object is null");
        }

        if (notification.Data is null)
        {
            throw new Exception("Notification data object is null");
        }
        
        return new ResourceIdentifier($"/subscriptions/{notification.Data.SubscriptionId}");;
    }

    /// <summary>
    /// Check Subscription Tags
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="credential"></param>
    /// <param name="resourceId"></param>
    private static bool ExclusionTagExists(ILogger logger, TokenCredential credential, ResourceIdentifier resourceId)
    {
        logger.LogDebug("[HttpTrigger] Checking subscription tags");

        var armClient = new ArmClient(credential);
        var subscription = armClient.GetSubscription(resourceId);
        
        var enumerator = subscription.GetAllPredefinedTags().GetEnumerator();

        try
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is not {TagName: "Exclusion"}) continue;
                logger.LogDebug("[HttpTrigger] Subscription exclusion found");
                return true;
            }
        }
        catch (RequestFailedException fe)
        {
            var messages = fe.Message.Split(
                new [] { Environment.NewLine },
                StringSplitOptions.None
            );
            
            throw new Exception(messages[0]);
        }
        catch
        {
            enumerator.Dispose();
        }

        return false;
    }
    
    /// <summary>
    /// Disable subscription
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="credential"></param>
    /// <param name="resourceId"></param>
    private static async Task DisableSubscription(ILogger logger, TokenCredential credential, ResourceIdentifier resourceId)
    {
        logger.LogDebug("[HttpTrigger] Disabling subscription");
        AccessToken token;

        try
        {
            token = await credential.GetTokenAsync(new TokenRequestContext(), CancellationToken.None);
        }
        catch
        {
            throw new Exception("Exception caught..");
        }
        
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://management.azure.com");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        
    }
}
