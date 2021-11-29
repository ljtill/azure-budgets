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

- [x] Update Folder Structure
- [x] Create Bicep / ARM files for deployment
- [ ] Add workflow to package up release
- [ ] Simplify Getting Started doc in README.md
- [ ] Deploy Function App code after infra/
- [ ] Rename Screenshots for each section
- [ ] Check for Tag Exclusion
- [ ] Write Table Storage Logs
- [ ] Create Getting Started guide
- [ ] Create Portal deployment experience
