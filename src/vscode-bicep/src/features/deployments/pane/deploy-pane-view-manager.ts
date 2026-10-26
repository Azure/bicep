// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { commands, ExtensionContext, Uri, ViewColumn, WebviewPanel, WebviewPanelSerializer, window } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { DiagnosticsRouter } from "../../../infrastructure/language-client";
import { Disposable } from "../../../infrastructure/lifecycle";
import { Prompts } from "../../../infrastructure/prompts";
import { IAzureUIManager } from "../azure/azure-ui-manager";
import { DeployPaneView } from "./deploy-pane-view";

export class DeployPaneViewManager extends Disposable implements WebviewPanelSerializer {
  private static readonly deployPaneActiveContextKey = "deployPaneFocus";

  private readonly viewsByPath = new Map<string, DeployPaneView>();

  private activeUri: Uri | undefined = undefined;

  constructor(
    private readonly prompts: Prompts,
    private readonly extensionContext: ExtensionContext,
    private readonly extensionUri: Uri,
    private readonly languageClient: LanguageClient,
    private readonly azureMgr: IAzureUIManager,
    diagnosticsRouter: DiagnosticsRouter,
  ) {
    super();

    this.register(window.registerWebviewPanelSerializer(DeployPaneView.viewType, this));
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
      DeployPaneView.create(
        this.extensionContext,
        this.prompts,
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

  public async deserializeWebviewPanel(webviewPanel: WebviewPanel, documentPath: string): Promise<void> {
    const documentUri = Uri.file(documentPath);

    this.registerView(
      documentUri,
      DeployPaneView.revive(
        this.extensionContext,
        this.prompts,
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

  private registerView(documentUri: Uri, view: DeployPaneView): DeployPaneView {
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
    await commands.executeCommand("setContext", DeployPaneViewManager.deployPaneActiveContextKey, value);
  }
}
