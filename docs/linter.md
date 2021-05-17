# Linter

The Bicep linter will inspect your code and catch a customizable set of authoring best practices. It will surface warnings, errors, or informational messages as you're typing in VS Code for immediate feedback. This means you don't have to build or deploy your code to find out that you've made a mistake. Some rules can also surface an automatic code fix through the VS Code light bulb. The linter should make it easier to enforce team coding standards by providing guidance during the development inner loop, as well as the ability to break a build during continuous integration (CI) if there are violations.

TO DO: ADD DEMO GIF HERE

## Configuration

[`bicepsettings.json`](./src/Bicep.Core/Configuration/bicepsettings.json) can be used to:

- enable/disable analyzers
- set DiagnosticLevel of rules
  - Warning (default for core rules) - [Displays a warning](https://code.visualstudio.com/docs/editor/editingevolved#_errors-warnings) (caution symbol) and does not prevent building (transpilation)
  - Error - [Displays an error](https://code.visualstudio.com/docs/editor/editingevolved#_errors-warnings) and throws an error at build time
  - Info - Displays an info message and does not prevent building (transpilation)
  - Off - Disables the rule
- set rule-specific values

`bicepsettings.json` can be placed alongside your templates in the same directory. The closest configuration file found up the tree will be used.

There are a set of core rules that are enabled by default. You can find their descriptions in the [`./rules`](./rules) folder.

## Future

The linter is being designed to be extensible so new rules can be added by either the Bicep team or the community. In the 0.5 milestone, we will be focusing more on extensibility, making it as easy as possible to contribute new rules and/or analyzers.

If you have an idea for a new rule, please use the ```linting-rule``` GitHub label: [![linter-rule](https://img.shields.io/github/issues/Azure/Bicep/linting-rule?color=important&label=linting-rule)](https://github.com/Azure/Bicep/issues?q=is%3Aissue+is%3Aopen+label%3A%22linting-rule%22)

## ARM template test toolkit (ARM-TTK)

We've ported over the rules from the [ARM template test toolkit](https://docs.microsoft.com/azure/azure-resource-manager/templates/test-toolkit) that make sense for Bicep. We recommend using the Bicep linter when working with Bicep, and the ARM template test toolkit when working with ARM templates
