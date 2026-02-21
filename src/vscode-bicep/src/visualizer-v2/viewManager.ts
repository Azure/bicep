// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../utils/disposable";
import { VisualDesignerView } from "./view";

export class VisualDesignerViewManager extends Disposable implements vscode.WebviewPanelSerializer {
  private static readonly visualizerActiveContextKey = "bicepVisualizerFocus";

  private readonly viewsByPath = new Map<string, VisualDesignerView>();

  private activeUri: vscode.Uri | undefined = undefined;

  constructor(
    private readonly extensionUri: vscode.Uri,
    private readonly languageClient: LanguageClient,
  ) {
    super();

    this.register(vscode.window.registerWebviewPanelSerializer(VisualDesignerView.viewType, this));

    const existingMiddleware = languageClient.clientOptions.middleware?.handleDiagnostics;

    this.languageClient.clientOptions.middleware = {
      ...(this.languageClient.clientOptions.middleware ?? {}),
      handleDiagnostics: (uri, diagnostics, next) => {
        for (const view of this.viewsByPath.values()) {
          view.render();
        }

        if (existingMiddleware) {
          existingMiddleware(uri, diagnostics, next);
        } else {
          next(uri, diagnostics);
        }
      },
    };
  }

  get activeDocumentUri(): vscode.Uri | undefined {
    return this.activeUri;
  }

  public async openView(documentUri: vscode.Uri, viewColumn: vscode.ViewColumn): Promise<void> {
    const existingView = this.viewsByPath.get(documentUri.fsPath);

    if (existingView) {
      existingView.reveal();
      return;
    }

    this.registerView(
      documentUri,
      VisualDesignerView.create(this.languageClient, viewColumn, this.extensionUri, documentUri),
    );

    await this.setVisualizerActiveContext(true);
    this.activeUri = documentUri;
  }

  public async deserializeWebviewPanel(webviewPanel: vscode.WebviewPanel, documentPath: string): Promise<void> {
    const documentUri = vscode.Uri.file(documentPath);

    this.registerView(
      documentUri,
      VisualDesignerView.revive(this.languageClient, webviewPanel, this.extensionUri, documentUri),
    );
  }

  public dispose(): void {
    super.dispose();

    this.languageClient.clientOptions.middleware = {
      ...this.languageClient.clientOptions.middleware,
      handleDiagnostics: undefined,
    };

    for (const view of this.viewsByPath.values()) {
      view.dispose();
    }

    this.viewsByPath.clear();
  }

  private registerView(documentUri: vscode.Uri, view: VisualDesignerView): VisualDesignerView {
    this.viewsByPath.set(documentUri.fsPath, view);

    view.onDidChangeViewState((e) => {
      void this.setVisualizerActiveContext(e.webviewPanel.active);
      if (e.webviewPanel.active) {
        this.activeUri = documentUri;
        view.render();
      }
    });

    view.onDidDispose(async () => {
      if (this.activeUri === documentUri) {
        void this.setVisualizerActiveContext(false);
        this.activeUri = undefined;
      }

      this.viewsByPath.delete(documentUri.fsPath);
    });

    return view;
  }

  private async setVisualizerActiveContext(value: boolean) {
    await vscode.commands.executeCommand("setContext", VisualDesignerViewManager.visualizerActiveContextKey, value);
  }
}
