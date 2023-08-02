# Using the Deployment Pane

## What is it?
The Deployment Pane is a UI panel in VSCode that allows you to connect to your Azure subscription and execute validate, deploy & whatif operations and get instant feedback without leaving the editor.

This feature is currently experimental while we collect feedback, but can be easily enabled if desired.

## Enabling
1. You must be using Bicep VSCode extension v0.20.4 or later.
1. You must enable the "Experimental: Deploy Pane" VSCode setting before you can access the functionality.

## Using
1. Ensure you've followed the steps under [Enabling](#enabling).
1. Open a `.bicep` or `.bicepparam` file in your editor.
1. Press the Deploy Pane button visible in the top right of your editor window.
1. Select a deployment scope with the "Pick Scope" button. This is only necessary if you haven't previously configured it for a given file, or wish to change the scope.
1. (if using a `.bicep` file) Select a JSON parameters file, or manually enter the parameters.
1. (if using a `.bicepparam` file) Change the parameter values if necessary.
1. Press either the `Validate`, `Deploy` or `WhatIf` buttons to submit your deployment to Azure and view results.

## Limitations / Notes
1. Only subscription & resource group scoped deployments are currently supported.
1. Changes you make in the editor are picked up immediately - you do not need to save the file.

## Raising bugs or feature requests
Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
