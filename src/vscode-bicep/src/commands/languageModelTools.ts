// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { registerLMTool } from "@microsoft/vscode-azext-utils";
import { LanguageClient } from "vscode-languageclient/node";
import { contributes } from "../../package.json";
import { languageModelToolInvokeRequestType } from "../language";
import { LanguageModelTextPart, LanguageModelToolResult } from "vscode";

export function registerLanguageModelTools(languageClient: LanguageClient): void {
  for (const tool of contributes.languageModelTools) {
    registerLMTool(
      tool.name,
      {
        invoke: async (_, options, token) => {
          const response = await languageClient.sendRequest(languageModelToolInvokeRequestType, {
            toolName: tool.name,
            inputJson: JSON.stringify(options.input),
          }, token);

          return new LanguageModelToolResult([
            new LanguageModelTextPart(response.content)
          ]);
        },
      });
  }
}