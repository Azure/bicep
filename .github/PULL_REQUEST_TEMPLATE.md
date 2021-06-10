# Contributing a Pull Request

If you haven't already, read the full [contribution guide](../CONTRIBUTING.md). The guide may have changed since the last time you read it, so please double-check. Once you are done and ready to submit your PR, run through the relevant checklist below.

## Contributing to documentation

* [ ] The contribution does not exist in any of the docs in either the root of the [docs](../docs) directory or the [specs](../docs/spec)

## Contributing an example

We are integrating the Bicep examples into the [Azure QuickStart Templates](https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/README.md).  If you'd like to contribute new example `.bicep` files that showcase abilities of the language, please follow [these instructions](https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/README.md) to add them directly there.  We can still take bug reports and fixes for the existing examples for the time being.

* [ ] This is a bug fix for an existing example
* [ ] I have resolved all warnings and errors shown by the Bicep VS Code extension
* [ ] I have checked that all tests are passing by running `dotnet test`
* [ ] I have consistent casing for all of my identifiers and am using camelCasing unless I have a justification to use another casing style

## Contributing a feature

* [ ] I have opened a new issue for the proposal, or commented on an existing one, and ensured that the Bicep maintainers are good with the design of the feature being implemented
* [ ] I have included "Fixes #{issue_number}" in the PR description, so GitHub can link to the issue and close it when the PR is merged
* [ ] I have appropriate test coverage of my new feature

## Contributing a snippet

* [ ] I have a snippet that is either a single, generic resource or multi resource that uses [parent-child syntax](https://github.com/Azure/bicep/blob/a22b9c80ba4f8b977f5d948f8bd8c54155ff6870/docs/spec/resource-scopes.md#parent-child-syntax)
* [ ] I have checked that there is not an equivalent snippet already submitted
* [ ] I have used camelCasing unless I have a justification to use another casing style
* [ ] I have placeholders values that correspond to their property names (e.g. `dnsPrefix: 'dnsPrefix'`), unless it's a property that MUST be changed or parameterized in order to deploy. In that case, I use 'REQUIRED' e.g. [keyData](./src/Bicep.LangServer/Snippets/Templates/res-aks-cluster.bicep#L26)
* [ ] I have my symbolic name as the first tab stop ($1) in the snippet. e.g. [res-aks-cluster.bicep](./src/Bicep.LangServer/Snippets/Templates/res-aks-cluster.bicep)
* [ ] I have a resource name property equal to "name"

  e.g.

  ```bicep
  resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
    name: 'name'
  ```
