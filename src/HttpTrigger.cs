using Budgets.Exceptions;
using Budgets.Models;

namespace Budgets;

public class HttpTrigger
{
    private readonly ILogger _logger;
    private string subscriptionId;

    public HttpTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpTrigger>();
    }

    [Function("HttpTrigger")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
            _logger.LogDebug("Parsing function request.");
            subscriptionId = ParseHttpRequest(req).Data.SubscriptionId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing function request.");
            throw;
        }

        try
        {
            _logger.LogDebug("Initializing subscription client.");
            var client = new ArmClient(new DefaultAzureCredential());
            //var subscription = client.GetSubscription(new ResourceIdentifier($"/subscriptions/{subscriptionId}"));

            // _logger.LogDebug("Checking subscription tags.");
            // var tags = subscription.GetPredefinedTags().GetAll();
        }
        catch (Exception e)
        {
            _logger.LogError($"Undefined exception: {e.Message}");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }

        return req.CreateResponse(HttpStatusCode.OK);
    }

    /// <summary>
    /// Parses the http request data.
    /// </summary>
    private static Alert ParseHttpRequest(HttpRequestData req)
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
        return JsonSerializer.Deserialize<Alert>(body, options);
    }

}
