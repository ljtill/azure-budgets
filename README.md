# Budgets

Budgets is a tool for enforcing hard limits on Azure Subscriptions spend. By integrating with Cost Management and Action Groups this tool can pro-actively disable a given Subscription when defined limits have been reached.

## Tasks

- [x] Create Folder Structure
- [x] Rename `HttpTrigger1`
- [x] Create Bicep files for deployment
- [ ] Write Table Storage Logs
- [ ] Check for Tag Exclusion
- [ ] Create Getting Started guide
- [ ] Create Portal deployment experience

## Structure

- docs/ - Documentation
- eng/ - Tooling (inc Templates)
- src/ - Source Code

## Getting Started

TODO

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

The Common Alert Schema is currently [unsupported](https://docs.microsoft.com/en-us/azure/azure-monitor/alerts/alerts-common-schema#how-do-i-enable-the-common-alert-schema) for Cost Management
