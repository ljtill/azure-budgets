namespace Microsoft.AppInnovation.Budgets.Schemas
{
    public class Alert
    {
        public Alert() { }
        public string SchemaId { get; set; }
        public AlertData Data { get; set; }
    }

    public class AlertData
    {
        public AlertData() { }
        public string SubscriptionName { get; set; }
        public string SubscriptionId { get; set; }
        public string SpendingAmount { get; set; }
        public string BudgetStartDate { get; set; }
        public string Budget { get; set; }
        public string Unit { get; set; }
        public string BudgetCreator { get; set; }
        public string BudgetName { get; set; }
        public string BudgetType { get; set; }
        public string ResourceGroup { get; set; }
        public string NotificationThresholdAmount { get; set; }
    }
}