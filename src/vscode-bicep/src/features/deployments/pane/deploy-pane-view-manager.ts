// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext } from "../../../infrastructure/action-context";
import { DiagnosticsRouter } from "../../../infrastructure/language-client";
import { Disposable } from "../../../infrastructure/lifecycle";
import { IAzureUIManager } from "../azure/azure-ui-manager";
import { DeployPaneView } from "./deploy-pane-view";

export class DeployPaneViewManager extends Disposable implements vscode.WebviewPanelSerializer {
  private static readonly deployPaneActiveContextKey = "deployPaneFocus";

  private readonly viewsByPath = new Map<string, DeployPaneView>();

  private activeUri: vscode.Uri | undefined = undefined;

  constructor(
    private readonly context: IActionContext,
    private readonly extensionContext: vscode.ExtensionContext,
    private readonly extensionUri: vscode.Uri,
    private readonly languageClient: LanguageClient,
    private readonly azureMgr: IAzureUIManager,
    diagnosticsRouter: DiagnosticsRouter,
  ) {
    super();

    this.register(vscode.window.registerWebviewPanelSerializer(DeployPaneView.viewType, this));
    this.register(
      diagnosticsRouter.subscribe(() => {
        for (const view of this.viewsByPath.values()) {
          view.render();
        }
      }),
    );
  }

  get activeDocumentUri(): vscode.Uri | undefined {
    return this.activeUri;
  }

  public async openView(documentUri: vscode.Uri, viewColumn: vscode.ViewColumn): Promise<void> {
    const existingView = this.viewsByPath.get(documentUri.fsPath);

    if (existingView) {
      existingView.reveal();
      await existingView.waitUntilReady();
      return;
    }

    const view = this.registerView(
      documentUri,
      DeployPaneView.create(
        this.extensionContext,
        this.context,
        this.azureMgr,
        this.languageClient,
        viewColumn,
        this.extensionUri,
        documentUri,
      ),
    );

    await this.setDeployPaneActiveContext(true);
    this.activeUri = documentUri;
    await view.waitUntilReady();
  }

  public async deserializeWebviewPanel(webviewPanel: vscode.WebviewPanel, documentPath: string): Promise<void> {
    const documentUri = vscode.Uri.file(documentPath);

    this.registerView(
      documentUri,
      DeployPaneView.revive(
        this.extensionContext,
        this.context,
        this.azureMgr,
        this.languageClient,
        webviewPanel,
        this.extensionUri,
        documentUri,
      ),
    );
  }

  public dispose(): void {
    super.dispose();

    for (const view of this.viewsByPath.values()) {
      view.dispose();
    }

    this.viewsByPath.clear();
  }

  private registerView(documentUri: vscode.Uri, view: DeployPaneView): DeployPaneView {
    this.viewsByPath.set(documentUri.fsPath, view);

    view.onDidChangeViewState((e) => {
      // Don't wait
      void this.setDeployPaneActiveContext(e.webviewPanel.active);
      if (e.webviewPanel.active) {
        this.activeUri = documentUri;
        view.render();
      }
    });

    view.onDidDispose(() => {
      if (this.activeUri === documentUri) {
        // Don't wait
        void this.setDeployPaneActiveContext(false);
        this.activeUri = undefined;
      }

      this.viewsByPath.delete(documentUri.fsPath);
    });

    return view;
  }

  private async setDeployPaneActiveContext(value: boolean) {
    await vscode.commands.executeCommand("setContext", DeployPaneViewManager.deployPaneActiveContextKey, value);
  }
}
