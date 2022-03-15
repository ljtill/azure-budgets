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
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
            var resourceId = GetResourceId(_logger, req);
            if (ExclusionTagExists(_logger, resourceId) is false)
            {
                DisableSubscription(_logger, resourceId);
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
    /// Parse Alert
    /// </summary>
    private static string GetResourceId(ILogger logger, HttpRequestData req)
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
        
        return $"/subscriptions/{notification.Data.SubscriptionId}";
    }

    /// <summary>
    /// Check Subscription Tags
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="resourceId"></param>
    private static bool ExclusionTagExists(ILogger logger, string resourceId)
    {
        logger.LogDebug("[HttpTrigger] Checking subscription tags");
        
        return false;
    }
    
    /// <summary>
    /// Disable Subscription
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="resourceId"></param>
    private static void DisableSubscription(ILogger logger, string resourceId)
    {
        logger.LogDebug("[HttpTrigger] Disabling subscription");
    }
}
