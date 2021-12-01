using System.Collections.Generic;

namespace Microsoft.AppInnovation.Budgets.Schemas
{
    public class SubscriptionTags
    {
        public SubscriptionTags() { }
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public Properties() { }
        public Dictionary<string, string> tags { get; set; }
    }
}