namespace Budgets.Models;

public class Subscription
{
    public Subscription(string id, string authorizationSource, string[] managedByTenants, string subscriptionId, string tenantId, string displayName, string state, SubscriptionPolicies subscriptionPolicies)
    {
        Id = id;
        AuthorizationSource = authorizationSource;
        ManagedByTenants = managedByTenants;
        SubscriptionId = subscriptionId;
        TenantId = tenantId;
        DisplayName = displayName;
        State = state;
        SubscriptionPolicies = subscriptionPolicies;
    }
    public string Id { get; }
    public string AuthorizationSource { get; }
    public string[] ManagedByTenants { get; }
    public string SubscriptionId { get; }
    public string TenantId { get; }
    public string DisplayName { get; }
    public string State { get; }
    public SubscriptionPolicies SubscriptionPolicies { get; }
}

public class SubscriptionPolicies
{
    public SubscriptionPolicies(string locationPlacementId, string quotaId, string spendingLimit)
    {
        LocationPlacementId = locationPlacementId;
        QuotaId = quotaId;
        SpendingLimit = spendingLimit;
    }
    public string LocationPlacementId { get; }
    public string QuotaId { get; }
    public string SpendingLimit { get; }
}
