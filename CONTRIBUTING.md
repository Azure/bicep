# Contributing to Bicep
We are very happy to accept community contributions to Bicep, whether those are [Pull Requests](#pull-requests), [Example Files](#example-files), [Feature Suggestions](#feature-suggestions) or [Bug Reports](#bug-reports)! Please note that by participating in this project, you agree to abide by the [Code of Conduct](./CODE_OF_CONDUCT.md), as well as the terms of the [CLA](#cla).

## Getting Started
* If you haven't already, you will need [dotnet](https://dotnet.microsoft.com/download) core sdk 3.1 (or later) and [node + npm](https://nodejs.org/en/download/) 10 (or later) installed locally to build and run this project.
* You are free to work on Bicep on any platform using any editor, but you may find it quickest to get started using [VSCode](https://code.visualstudio.com/Download) with the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp).
* Fork this repo (see [this forking guide](https://guides.github.com/activities/forking/) for more information).
* Checkout the repo locally with `git clone git@github.com:{your_username}/bicep.git`.
* Build the .NET solution with `dotnet build`. If this fails try running `git submodule update --init --recursive` first.

## Developing
### Components
The Bicep solution is comprised of the following main components:
* **Bicep CLI** (`src/Bicep.Cli`): the `bicep` CLI exectuable.
* **Bicep Language Server** (`src/Bicep.LangServer`): the LanguageServer used by the VSCode extension for parsing and providing information about a Bicep file.
* **Bicep Core** (`src/Bicep.Core`): the library containing the majority of the Bicep compiler code.
* **Bicep VSCode Extension** (`src/vscode-bicep`): the VSCode extension itself. This is mostly a thin wrapper around the Bicep Language Server.
* **Playground** (`src/playground`): the web-based playground hosted at `https://aka.ms/bicepdemo`.
* A number of different test suites.

### Running the tests
* You can use the following command to run the full Bicep test suite:
    * `dotnet test`

### Updating test baselines
* Many of the bicep integration tests rely on baseline test assertion files that are checked into the repo. Code changes in some areas will require updates to the baseline assertions. 
* If you see a test failure with a message containing Windows and *nix copy commands, you have encountered such a test. You have the following options to fix the test:
  1. Manually execute the provided command in a shell. This makes sense for a single test, but is extremely tedious otherwise.
  1. Run the `SetBaseline.ps1` script at the repo root to execute the tests in `SetBaseLine` mode, which causes the baselines to be automatically updated in bulk for failing tests. You should see baseline file modifications in Git pending changes. (Make sure your Git pending changes are empty before doing so - your changes could get overwritten!).
* Inspect the baseline assertion diffs to ensure changes are expected and match the code changes you have made. (If a pull request contains changes to baseline files that can't be explained, it will not be merged.)

### Running the Bicep VSCode extension
* On the first run, you'll need to ensure you have installed all the npm packages required by the Bicep VSCode extension with the following:
    * `cd src/vscode-bicep`
    * `npm i`
* In the [VSCode Run View](https://code.visualstudio.com/Docs/editor/debugging), select the "Bicep VSCode Extension" task, and press the "Start" button. This will launch a new VSCode window with the Bicep extension and LanguageServer containing your changes. When running on WSL, set the `BICEP_LANGUAGE_SERVER_PATH` environment variable found in `launch.json` manually following [WSL environment setup scripts](https://code.visualstudio.com/docs/remote/wsl#_advanced-environment-setup-script).
* If you want the ability to put breakpoints and step through the C# code, you can also use the "Attach" run configuration once the extension host has launched, and select the Bicep LanguageServer process by searching for "bicep".

### Running the Bicep CLI
* In the [VSCode Run View](https://code.visualstudio.com/Docs/editor/debugging), select the "Bicep CLI" task, and press the "Start" button. This will build and run the Bicep CLI and allow you to step through the code.
* Note that usually you will want to pass your own custom arguments to the Bicep CLI. This can be done by modifying the `launch.json` configuration to add arguments to the "args" array for the "Bicep CLI" task. 

### Running the Playground
* On the first run, you'll need to ensure you have installed all the npm packages required by the Bicep Playground with the following:
    * `cd src/playground`
    * `npm i`
* In the [VSCode Run View](https://code.visualstudio.com/Docs/editor/debugging), select the "Bicep Playground" task, and press the "Start" button. This will launch a browser window with the Playground containing your changes.

## Pull Requests
If you'd like to start contributing to Bicep, you can search for issues tagged as "good first issue" [here](https://github.com/Azure/bicep/labels/good%20first%20issue).

### Bicep Code
* Ensure that an issue has been created to track the feature enhancement or bug that is being fixed.
* In the PR description, make sure you've included "Fixes #{issue_number}" e.g. "Fixes #242" so that GitHub knows to link it to an issue.
* To avoid multiple contributors working on the same issue, please add a comment to the issue to let us know you plan to work on it.
* If a significant amount of design is required, please include a proposal in the issue and wait for approval before working on code. If there's anything you're not sure about, please feel free to discuss this in the issue. We'd much rather all be on the same page at the start, so that there's less chance that drastic changes will be needed when your pull request is reveiwed.
* We report on code coverage; please ensure any new code you add is sufficiently covered by tests.

### Example Files
If you'd like to contribute example `.bicep` files that showcase abilities of the language:
* Create an appropriately-named directory inside `docs/examples`. Note that the directory naming matches that of the [azure quickstart template repo](https://github.com/Azure/azure-quickstart-templates).
* Include your file named `main.bicep`.
* Compile the file using the Bicep CLI, and include the compiled `main.json` with your check-in.
* Pull Request validation will ensure that the `main.bicep` file can be compiled successfully without errors, and that the generated `main.json` file matches the one being checked in. If you want to validate that this will pass locally before submitting a PR, you can use `dotnet test` in the root directory of this repo to run the full test suite.
* While everything will *not necessarily be applicable*, read through the Azure QuickStart Templates [Best Practices Guide](https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/best-practices.md#best-practices) and follow it where appropriate (i.e. [parameter guidance](https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/best-practices.md#parameters), [resource property order](https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/best-practices.md#sort-order-of-properties), etc.)

**Note:** If you have never submitted a Pull Request or used git before, reading through the [Git tutorial](https://github.com/Azure/azure-quickstart-templates/blob/master/1-CONTRIBUTION-GUIDE/git-tutorial.md) in the azure-quickstart-template repo is a good place to start.

## Feature Suggestions
* Please first search [Open Bicep Issues](https://github.com/Azure/bicep/issues) before opening an issue to check whether your feature has already been suggested. If it has, feel free to add your own comments to the existing issue.
* Ensure you have included a "What?" - what your feature entails, being as specific as possible, and giving mocked-up syntax examples where possible.
* Ensure you have included a "Why?" - what the benefit of including this feature will be.
* Use the "Feature Request" issue template [here](https://github.com/Azure/bicep/issues/new/choose) to submit your request.

## Bug Reports
* Please first search [Open Bicep Issues](https://github.com/Azure/bicep/issues) before opening an issue, to see if it has already been reported.
* Try to be as specific as possible, including the version of the Bicep CLI or extension used to reproduce the issue, and any example files or snippets of Bicep code needed to reproduce it.
* Use the "Bug Report" issue template [here](https://github.com/Azure/bicep/issues/new/choose) to submit your request.

## CLA
This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
