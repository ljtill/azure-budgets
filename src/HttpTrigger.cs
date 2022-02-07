using System.Threading;
using Budgets.Exceptions;
using Budgets.Models;

namespace Budgets;

public class HttpTrigger
{
    private readonly ILogger _logger;
    private readonly ArmClient _client;
    private ResourceIdentifier _resourceId;

    public HttpTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpTrigger>();
        _client = new ArmClient(new DefaultAzureCredential());
    }

    [Function("HttpTrigger")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
            _resourceId = GetResourceId(_logger, req);
            var subscription = _client.GetSubscription(_resourceId);
            if (ExclusionTagExists(_logger, subscription) is false)
            {
                DisableSubscription(_logger, _resourceId);
            }
        }
        catch (Exception ex)
        {
            // TODO: Exception (RBAC) Handler
            _logger.LogError(ex.Message);
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }

        return req.CreateResponse(HttpStatusCode.OK);
    }

    /// <summary>
    /// Initialize Resource Id from HTTP Request body
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

        var alert = JsonSerializer.Deserialize<Notification>(body, options);
        if (alert is null)
        {
            throw new Exception("Invalid input value");
        }
        
        return new ResourceIdentifier($"/subscriptions/{alert.Data.SubscriptionId}");;
    }

    /// <summary>
    /// Check if Subscription exclusion tag exists
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="subscription"></param>
    private static bool  ExclusionTagExists(ILogger logger, Subscription subscription)
    {
        logger.LogDebug("[HttpTrigger] Checking subscription tags");
        
        var tagExists = false;
        var enumerator = subscription.GetAllPredefinedTags().GetEnumerator();
        
        try
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is not { TagName: "Exclusion"} ) continue;
                logger.LogInformation("[HttpTrigger] Subscription exclusion found");
                tagExists = true;
            }
        }
        catch
        {
            enumerator.Dispose();
        }

        return tagExists;
    }

    /// <summary>
    /// Disable subscription
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="resourceId"></param>
    private static void DisableSubscription(ILogger logger, ResourceIdentifier resourceId)
    {
        logger.LogDebug("[HttpTrigger] Disabling subscription");
        // TODO: Create REST Operation
        // Parameter (IgnoreResourceCheck=true)
        // DefaultCredential().GetToken();
    }
}
