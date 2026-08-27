// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { commands, Uri, ViewColumn, WebviewPanel, WebviewPanelSerializer, window, workspace } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { DiagnosticsRouter } from "../../infrastructure/language-client";
import { Disposable } from "../../infrastructure/lifecycle";
import { getLogger } from "../../infrastructure/logging";
import { BicepVisualizerView } from "./visualizer-view";
import { resourceCreationSetting } from "./resource-creation-setting";

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

    this.registerMultiple(
      window.registerWebviewPanelSerializer(BicepVisualizerView.viewType, this),
      workspace.onDidChangeTextDocument((event) => {
        this.viewsByPath.get(event.document.uri.fsPath)?.render();
      }),
      workspace.onDidChangeConfiguration((event) => {
        if (event.affectsConfiguration("workbench.reduceMotion")) {
          for (const view of this.viewsByPath.values()) {
            view.notifyMotionPolicyDidChange();
          }
        }
        if (event.affectsConfiguration(`bicep.${resourceCreationSetting}`)) {
          for (const view of this.viewsByPath.values()) {
            view.notifyResourceCreationEnablementDidChange();
          }
        }
      }),
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
      existingView.reveal(viewColumn);
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

  public async deserializeWebviewPanel(webviewPanel: WebviewPanel, state: unknown): Promise<void> {
    const documentPath =
      typeof state === "string"
        ? state
        : typeof state === "object" &&
            state !== null &&
            "documentPath" in state &&
            typeof state.documentPath === "string"
          ? state.documentPath
          : undefined;

    if (!documentPath) {
      getLogger().warn("Could not restore Bicep visualizer because its serialized document path is missing.");
      webviewPanel.dispose();
      return;
    }

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

    view.onDidDispose(() => {
      if (this.activeUri === documentUri) {
        void this.setVisualizerActiveContext(false);
        this.activeUri = undefined;
      }

      if (this.viewsByPath.get(documentUri.fsPath) === view) {
        this.viewsByPath.delete(documentUri.fsPath);
      }
    });

    return view;
  }

  private async setVisualizerActiveContext(value: boolean) {
    await commands.executeCommand("setContext", BicepVisualizerViewManager.visualizerActiveContextKey, value);
  }
}
