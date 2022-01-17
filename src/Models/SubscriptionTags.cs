namespace Budgets.Models;

public class SubscriptionTags
{
    public SubscriptionTags()
    {
    }
    public SubscriptionTagsProperties Properties { get; set; }
}

public class SubscriptionTagsProperties
{
    public SubscriptionTagsProperties()
    {
    }
    public Dictionary<string, string> Tags { get; set; }
}