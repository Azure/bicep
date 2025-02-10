// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../utils/disposable";
import { decodeExternalSourceUri } from "./decodeExternalSourceUri";
import { BicepExternalSourceParams, bicepExternalSourceRequestType } from "./protocol";

export class BicepExternalSourceContentProvider extends Disposable implements vscode.TextDocumentContentProvider {
  constructor(private readonly languageClient: LanguageClient) {
    super();
  }

  onDidChange?: vscode.Event<vscode.Uri> | undefined;

  async provideTextDocumentContent(uri: vscode.Uri, token: vscode.CancellationToken): Promise<string> {
    // Ask the language server for the sources for the cached module
    const response = await this.languageClient.sendRequest(
      bicepExternalSourceRequestType,
      this.bicepExternalSourceRequest(uri),
      token,
    );

    return response.error ? `// ${response.error}` : (response.content ?? "");
  }

  private bicepExternalSourceRequest(uri: vscode.Uri): BicepExternalSourceParams {
    const { moduleReference, requestedSourceFile } = decodeExternalSourceUri(uri);
    return {
      target: moduleReference,
      requestedSourceFile,
    };
  }
}
