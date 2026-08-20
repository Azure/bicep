// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { commands, Uri, ViewColumn, WebviewPanel, WebviewPanelSerializer, window } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { DiagnosticsRouter } from "../../infrastructure/language-client";
import { Disposable } from "../../infrastructure/lifecycle";
import { BicepVisualizerView } from "./visualizer-view";

export class BicepVisualizerViewManager extends Disposable implements WebviewPanelSerializer {
  private static readonly visualizerActiveContextKey = "bicepVisualizerFocus";

  private readonly viewsByPath = new Map<string, BicepVisualizerView>();

  private activeUri: Uri | undefined = undefined;

  constructor(
    private readonly extensionUri: Uri,
    private readonly languageClient: LanguageClient,
    diagnosticsRouter: DiagnosticsRouter,
  ) {
    super();

    this.register(window.registerWebviewPanelSerializer(BicepVisualizerView.viewType, this));
    this.register(
      diagnosticsRouter.subscribe(() => {
        for (const view of this.viewsByPath.values()) {
          view.render();
        }
      }),
    );
  }

  get activeDocumentUri(): Uri | undefined {
    return this.activeUri;
  }

  public async openView(documentUri: Uri, viewColumn: ViewColumn): Promise<void> {
    const existingView = this.viewsByPath.get(documentUri.fsPath);

    if (existingView) {
      existingView.reveal();
      await existingView.waitUntilReady();
      return;
    }

    const view = this.registerView(
      documentUri,
      BicepVisualizerView.create(this.languageClient, viewColumn, this.extensionUri, documentUri),
    );

    await this.setVisualizerActiveContext(true);
    this.activeUri = documentUri;
    await view.waitUntilReady();
  }

  public async deserializeWebviewPanel(webviewPanel: WebviewPanel, documentPath: string): Promise<void> {
    const documentUri = Uri.file(documentPath);

    this.registerView(
      documentUri,
      BicepVisualizerView.revive(this.languageClient, webviewPanel, this.extensionUri, documentUri),
    );
  }

  public dispose(): void {
    super.dispose();

    for (const view of this.viewsByPath.values()) {
      view.dispose();
    }

    this.viewsByPath.clear();
  }

  private registerView(documentUri: Uri, view: BicepVisualizerView): BicepVisualizerView {
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
    await commands.executeCommand("setContext", BicepVisualizerViewManager.visualizerActiveContextKey, value);
  }
}
