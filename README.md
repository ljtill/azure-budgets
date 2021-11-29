# Budgets

Budgets is a tool for enforcing hard limits on Azure Subscriptions spend. By integrating with Cost Management and Action Groups this tool can pro-actively disable a given Subscription when defined limits have been reached.

## Guides

- [Getting Started](./docs/CONFIG.md)
- [Deployment](./docs/CONFIG.md###Deployment)
- [Permissions](./docs/CONFIG.md###Permissions)
- [Budgets](./docs/CONFIG.md###Budgets)
- [Schemas](./docs/SCHEMA.md)

## Structure

- docs/ - Documentation
- eng/ - Tooling (inc Templates)
- src/ - Source Code

## Development

- [x] (Docs) Update Folder Structure
- [ ] (Docs) Create Getting Started guide
- [ ] (Docs) Simplify Getting Started doc in README.md
- [ ] (Docs) Rename Screenshots for each section
- [x] (Code) Refactor identity approach
- [x] (Code) Create Bicep / ARM files for deployment
- [ ] (Code) Add workflow to package up release
- [ ] (Code) Write Table Storage Logs
- [ ] (Code) Deploy Function App code after infra/
- [ ] (Code) Check for Tag Exclusion
- [ ] (Code) Create Portal deployment experience
