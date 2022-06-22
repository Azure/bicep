// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// This is the expected new config whether created by snippet, light-bulb-edit a rule, or create new config command
//
// This is defined in two places in source code:
//   Snippet: bicep\src\vscode-bicep\vscode-snippets\jsonc-snippets.jsonc
//   Language server: bicep\src\Bicep.Core\Configuration\DefaultBicepConfigHelper.cs (without no-unused-params)

export const expectedNewConfigFileContents = `{
    // See https://aka.ms/bicep/config for more information on Bicep configuration options
    // Press CTRL+SPACE/CMD+SPACE at any location to see Intellisense suggestions
    "analyzers": {
        "core": {
            "rules": {
                "no-unused-params": {
                    "level": "warning"
                }
            }
        }
    }
}
`;
