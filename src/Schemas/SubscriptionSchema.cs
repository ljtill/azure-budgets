namespace Microsoft.AppInnovation.Budgets.Schemas
{
    public class Subscription
    {
        public Subscription() { }
        public string id { get; set; }
        public string authorizationSource { get; set; }
        public string[] managedByTenants { get; set; }
        public string subscriptionId { get; set; }
        public string tenantId { get; set; }
        public string displayName { get; set; }
        public string state { get; set; }
        public SubscriptionPolicies subscriptionPolicies { get; set; }
    }

    public class SubscriptionPolicies
    {
        public SubscriptionPolicies() { }
        public string locationPlacementId { get; set; }
        public string quotaId { get; set; }
        public string spendingLimit { get; set; }
    }
}