# Budgets

## Schemas

[Budget Schema](https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/cost-management-budget-scenario)

```json
{
    "schemaId": "AIP Budget Notification",
    "data": {
        "SubscriptionName": "CCM - Microsoft Azure Enterprise - 1",
        "SubscriptionId": "<GUID>",
        "SpendingAmount": "100",
        "BudgetStartDate": "6/1/2018",
        "Budget": "50",
        "Unit": "USD",
        "BudgetCreator": "email@contoso.com",
        "BudgetName": "BudgetName",
        "BudgetType": "Cost",
        "ResourceGroup": "",
        "NotificationThresholdAmount": "0.8"
    }
}
```

[Common Alert Schema](https://docs.microsoft.com/en-us/azure/azure-monitor/alerts/alerts-common-schema)

```text
The Common Alert Schema is currently [unsupported](https://docs.microsoft.com/en-us/azure/azure-monitor/alerts/alerts-common-schema#how-do-i-enable-the-common-alert-schema) for Cost Management
```