This repo tracks the design and implementation of the as-of-yet unnamed Azure Template Language Revision (codename Bicep - an ARM pun!). Bicep aims to provide customers with an intuitive and easy-to-use declarative programming model for configuring resources on Azure.

## High Level Design Goals
1. Code should be easy to understand at a glance and straightforward to learn, regardless of your experience with other programming languages.
2. The language should provide a 'transparent abstraction' for the underlying platform.
3. The language should feel familiar to imperative programmers, whilst making some of the declarative differences clear through syntax.
4. Users should be given a lot of freedom to modularize their code if they desire. There should be no need for 'copy/paste'.
5. Tooling should provide a high level of resource discoverability and validation, and should be developed alongside the compiler rather than added at the end.
6. Users should have a high level of confidence that their code is 'valid' before deploying.

## Contributing
This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
