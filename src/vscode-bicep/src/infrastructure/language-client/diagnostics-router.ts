// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Diagnostic, Disposable, Uri } from "vscode";
import type { LanguageClientOptions, Middleware } from "vscode-languageclient/node";

export type DiagnosticsSubscriber = (uri: Uri, diagnostics: Diagnostic[]) => void;

export class DiagnosticsRouter implements Disposable {
  private readonly subscribers = new Set<DiagnosticsSubscriber>();
  private readonly existingHandler: NonNullable<Middleware["handleDiagnostics"]> | undefined;

  public constructor(private readonly clientOptions: LanguageClientOptions) {
    this.existingHandler = clientOptions.middleware?.handleDiagnostics;
    this.clientOptions.middleware = {
      ...(this.clientOptions.middleware ?? {}),
      handleDiagnostics: (uri, diagnostics, next) => {
        for (const subscriber of this.subscribers) {
          subscriber(uri, diagnostics);
        }

        if (this.existingHandler) {
          this.existingHandler(uri, diagnostics, next);
        } else {
          next(uri, diagnostics);
        }
      },
    };
  }

  public subscribe(subscriber: DiagnosticsSubscriber): Disposable {
    this.subscribers.add(subscriber);

    return {
      dispose: () => this.subscribers.delete(subscriber),
    };
  }

  public dispose(): void {
    this.subscribers.clear();
    this.clientOptions.middleware = {
      ...this.clientOptions.middleware,
      handleDiagnostics: this.existingHandler,
    };
  }
}
