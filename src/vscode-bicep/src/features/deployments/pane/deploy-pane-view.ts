// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import crypto from "crypto";
import { readFile } from "fs/promises";
import path from "path";
import {
  Event,
  EventEmitter,
  ExtensionContext,
  TextDocument,
  Uri,
  ViewColumn,
  WebviewPanel,
  WebviewPanelOnDidChangeViewStateEvent,
  window,
  workspace,
} from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../../../infrastructure/lifecycle";
import { getLogger } from "../../../infrastructure/logging";
import { Prompts } from "../../../infrastructure/prompts";
import { debounce } from "../../../infrastructure/timing";
import { knownAzureResourceManagerEndpoints } from "../azure/azure-environment";
import { IAzureUIManager } from "../azure/azure-ui-manager";
import { getDeploymentDataRequestType, localDeployRequestType } from "../protocol";
import {
  createDeploymentDataMessage,
  createGetAccessTokenResultMessage,
  createGetDeploymentScopeResultMessage,
  createGetStateResultMessage,
  createLocalDeployResultMessage,
  createPickParamsFileResultMessage,
  ViewMessage,
} from "./messages";
import { DeployPaneState } from "./models";

const deployPaneStateKey = "bicep.deployPane.configState";

export class DeployPaneView extends Disposable {
  public static viewType = "bicep.deployPane";

  private readonly onDidDisposeEmitter: EventEmitter<void>;
  private readonly onDidChangeViewStateEmitter: EventEmitter<WebviewPanelOnDidChangeViewStateEvent>;
  private readonly ready: Promise<void>;
  private resolveReady!: () => void;

  private readyToRender = false;
  private document?: TextDocument;

  private constructor(
    private readonly extensionContext: ExtensionContext,
    private readonly prompts: Prompts,
    private readonly azureMgr: IAzureUIManager,
    private readonly languageClient: LanguageClient,
    private readonly webviewPanel: WebviewPanel,
    private readonly extensionUri: Uri,
    private readonly documentUri: Uri,
  ) {
    super();

    this.onDidDisposeEmitter = new EventEmitter<void>();
    this.onDidChangeViewStateEmitter = this.register(new EventEmitter<WebviewPanelOnDidChangeViewStateEvent>());
    this.ready = new Promise((resolve) => (this.resolveReady = resolve));

    this.register(this.webviewPanel.webview.onDidReceiveMessage(this.handleDidReceiveMessage, this));

    if (!this.isDisposed) {
      this.webviewPanel.webview.html = this.createWebviewHtml();
    }

    this.registerMultiple(
      this.webviewPanel.onDidDispose(this.dispose, this),
      this.webviewPanel.onDidChangeViewState((e) => this.onDidChangeViewStateEmitter.fire(e)),
    );
  }

  public get onDidDispose(): Event<void> {
    return this.onDidDisposeEmitter.event;
  }

  public get onDidChangeViewState(): Event<WebviewPanelOnDidChangeViewStateEvent> {
    return this.onDidChangeViewStateEmitter.event;
  }

  public static create(
    extensionContext: ExtensionContext,
    prompts: Prompts,
    azureMgr: IAzureUIManager,
    languageClient: LanguageClient,
    viewColumn: ViewColumn,
    extensionUri: Uri,
    documentUri: Uri,
  ): DeployPaneView {
    const visualizerTitle = `Deploy ${path.basename(documentUri.fsPath)}`;
    const webviewPanel = window.createWebviewPanel(DeployPaneView.viewType, visualizerTitle, viewColumn, {
      enableScripts: true,
      retainContextWhenHidden: true,
    });

    return new DeployPaneView(
      extensionContext,
      prompts,
      azureMgr,
      languageClient,
      webviewPanel,
      extensionUri,
      documentUri,
    );
  }

  public static revive(
    extensionContext: ExtensionContext,
    prompts: Prompts,
    azureMgr: IAzureUIManager,
    languageClient: LanguageClient,
    webviewPanel: WebviewPanel,
    extensionUri: Uri,
    documentUri: Uri,
  ): DeployPaneView {
    return new DeployPaneView(
      extensionContext,
      prompts,
      azureMgr,
      languageClient,
      webviewPanel,
      extensionUri,
      documentUri,
    );
  }

  public reveal(): void {
    this.webviewPanel.reveal();
  }

  public async waitUntilReady(): Promise<void> {
    await this.ready;
    if (this.isDisposed) {
      throw new Error("The deployment pane was disposed before it became ready.");
    }
  }

  public dispose(): void {
    super.dispose();

    this.webviewPanel.dispose();
    this.resolveReady();

    // Final cleanup.
    this.onDidDisposeEmitter.fire();
    this.onDidDisposeEmitter.dispose();
  }

  // Do "fire and forget" since there's no need to wait on rendering.
  public render = debounce(() => this.doRender());

  private async doRender() {
    if (this.isDisposed || !this.readyToRender) {
      return;
    }

    try {
      this.document = await workspace.openTextDocument(this.documentUri);
    } catch {
      this.webviewPanel.webview.html = this.createDocumentNotFoundHtml();
      return;
    }

    if (this.isDisposed) {
      return;
    }

    const deploymentData = await this.languageClient.sendRequest(getDeploymentDataRequestType, {
      textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(this.document),
    });

    if (this.isDisposed) {
      return;
    }

    try {
      await this.webviewPanel.webview.postMessage(
        createDeploymentDataMessage(
          this.documentUri.fsPath,
          deploymentData.localDeployEnabled,
          deploymentData.templateJson,
          deploymentData.parametersJson,
          deploymentData.errorMessage,
        ),
      );
    } catch (error) {
      // Race condition: the webview was closed before receiving the message,
      // which causes "Unknown webview handle" error.
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private async handleDidReceiveMessage(message: ViewMessage) {
    switch (message.kind) {
      case "READY": {
        getLogger().debug(`Deployment Pane for ${this.documentUri.fsPath} is ready.`);
        this.readyToRender = true;
        this.resolveReady();
        this.render();
        return;
      }
      case "GET_STATE": {
        const deployPaneState: Record<string, DeployPaneState> =
          this.extensionContext.globalState.get(deployPaneStateKey) || {};
        const filteredState = deployPaneState[this.documentUri.toString()];

        await this.webviewPanel.webview.postMessage(createGetStateResultMessage(filteredState));
        return;
      }
      case "SAVE_STATE": {
        const deployPaneState: Record<string, DeployPaneState> =
          this.extensionContext.globalState.get(deployPaneStateKey) || {};
        deployPaneState[this.documentUri.toString()] = message.state;

        await this.extensionContext.globalState.update(deployPaneStateKey, deployPaneState);
        return;
      }
      case "PICK_PARAMS_FILE": {
        const parametersFileUri = await this.prompts.showOpenDialog({
          canSelectMany: false,
          openLabel: "Select Parameters file",
          filters: { "Parameters files": ["json"] },
        });
        const parameterFile = await readFile(parametersFileUri[0].fsPath, "utf-8");
        await this.webviewPanel.webview.postMessage(
          createPickParamsFileResultMessage(parametersFileUri[0].fsPath, parameterFile),
        );
        return;
      }
      case "GET_ACCESS_TOKEN": {
        try {
          const accessToken = await this.azureMgr.getAccessToken(message.scope);

          await this.webviewPanel.webview.postMessage(createGetAccessTokenResultMessage(accessToken));
        } catch (error) {
          await this.webviewPanel.webview.postMessage(createGetAccessTokenResultMessage(undefined, error));
        }
        return;
      }
      case "GET_DEPLOYMENT_SCOPE": {
        const scope = await this.azureMgr.pickScope(message.scopeType);
        await this.webviewPanel.webview.postMessage(createGetDeploymentScopeResultMessage(scope));
        return;
      }
      case "LOCAL_DEPLOY": {
        const result = await this.languageClient.sendRequest(localDeployRequestType, {
          textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(this.document!),
        });

        await this.webviewPanel.webview.postMessage(createLocalDeployResultMessage(result));
        return;
      }
    }
  }

  private escapeHtml(value: string): string {
    return value
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#39;");
  }

  private createWebviewHtml() {
    const { cspSource } = this.webviewPanel.webview;
    const nonce = crypto.randomBytes(16).toString("hex");
    const scriptUri = this.webviewPanel.webview.asWebviewUri(
      Uri.joinPath(this.extensionUri, "out", "deploy-pane", "index.js"),
    );
    const codiconCssUri = this.webviewPanel.webview.asWebviewUri(
      Uri.joinPath(this.extensionUri, "out", "deploy-pane", "assets", "index.css"),
    );

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <!--
        Use a content security policy to only allow loading images from our extension directory,
        and only allow scripts that have a specific nonce.
        -->
        <meta http-equiv="Content-Security-Policy" content="default-src 'self' ${knownAzureResourceManagerEndpoints.join(" ")}; style-src ${cspSource} 'unsafe-inline'; img-src ${cspSource} data:; script-src 'nonce-${nonce}' vscode-webview-resource:; font-src data: ${cspSource};">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link id="vscode-codicon-stylesheet" rel="stylesheet" nonce="${nonce}" href="${codiconCssUri}">
      </head>
      <body>
        <div id="root"></div>
        <script nonce="${nonce}" type="module" src="${scriptUri}" />
      </body>
      </html>`;
  }

  private createDocumentNotFoundHtml() {
    const { cspSource } = this.webviewPanel.webview;
    const documentName = path.basename(this.documentUri.fsPath);
    const escapedDocumentName = this.escapeHtml(documentName);

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src ${cspSource} 'unsafe-inline'; img-src ${cspSource} data:; font-src data: ${cspSource};">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
      </head>
      <body>
        <div class="vscode-body">${escapedDocumentName} not found. It might be deleted or renamed.</div>
      </body>
      </html>`;
  }
}
