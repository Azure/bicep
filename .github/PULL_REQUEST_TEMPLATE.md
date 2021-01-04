# Contributing a Pull Request

If you haven't already, read the full [contribution guide](../CONTRIBUTING.md). The guide may have changed since the last time you read it, so please double-check. Once you are done and ready to submit your PR, run through the relevant checklist below.

## Contributing to documentation

* [ ] The contribution does not exist in any of the docs in either the root of the [docs](../docs) directory or the [specs](../docs/spec)

## Contributing an example

* [ ] I have checked that there is not an equivalent example already submitted
* [ ] I have resolved all warnings and errors shown by the Bicep VS Code extension
* [ ] I have checked that all tests are passing by running `dotnet test`
* [ ] I have consistent casing for all of my identifiers and am using camelCasing unless I have a justification to use another casing style

## Contributing a feature

* [ ] I have opened a new issue for the proposal, or commented on an existing one, and ensured that the bicep maintainers are good with the design of the feature being implemented
* [ ] I have included "Fixes #{issue_number}" in the PR description, so GitHub can link to the issue and close it when the PR is merged
* [ ] I have appropriate test coverage of my new feature
