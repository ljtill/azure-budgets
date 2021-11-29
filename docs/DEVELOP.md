# Budgets

## Development

Requirements

- Azure Functions Core Tools (Version X)
- Azure PowerShell (Version X)
- Azure CLI (Version X)
- Bicep (Version X)
- Azurite (Version X)

Create the _local.settings.json_ file

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AZURE_CLIENT_ID": "",
    "AZURE_CLIENT_SECRET": "",
    "AZURE_TENANT_ID": ""
  }
}
```
