// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { CancellationToken, Event, TextDocumentContentProvider, Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../../infrastructure/lifecycle";
import { decodeExternalSourceUri } from "./external-source-uri";
import { BicepExternalSourceParams, bicepExternalSourceRequestType } from "./protocol";

export class BicepExternalSourceContentProvider extends Disposable implements TextDocumentContentProvider {
  constructor(private readonly languageClient: LanguageClient) {
    super();
  }

  onDidChange?: Event<Uri> | undefined;

  async provideTextDocumentContent(uri: Uri, token: CancellationToken): Promise<string> {
    // Ask the language server for the sources for the cached module
    const response = await this.languageClient.sendRequest(
      bicepExternalSourceRequestType,
      this.bicepExternalSourceRequest(uri),
      token,
    );

    return response.error ? `// ${response.error}` : (response.content ?? "");
  }

  private bicepExternalSourceRequest(uri: Uri): BicepExternalSourceParams {
    const { moduleReference, requestedSourceFile } = decodeExternalSourceUri(uri);
    return {
      target: moduleReference,
      requestedSourceFile,
    };
  }
}
