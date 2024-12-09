// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import crypto from "crypto";
import path from "path";
import { Environment } from "@azure/ms-rest-azure-env";
import { callWithTelemetryAndErrorHandlingSync, IActionContext } from "@microsoft/vscode-azext-utils";
import fse from "fs-extra";
import vscode, { ExtensionContext } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { IAzureUiManager } from "../../azure/types";
import { GlobalStateKeys } from "../../globalState";
import { getDeploymentDataRequestType, localDeployRequestType } from "../../language";
import { Disposable } from "../../utils/disposable";
import { getLogger } from "../../utils/logger";
import { debounce } from "../../utils/time";
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

export class DeployPaneView extends Disposable {
  public static viewType = "bicep.deployPane";

  private readonly onDidDisposeEmitter: vscode.EventEmitter<void>;
  private readonly onDidChangeViewStateEmitter: vscode.EventEmitter<vscode.WebviewPanelOnDidChangeViewStateEvent>;

  private readyToRender = false;
  private document?: vscode.TextDocument;

  private constructor(
    private readonly extensionContext: ExtensionContext,
    private readonly context: IActionContext,
    private readonly azureMgr: IAzureUiManager,
    private readonly languageClient: LanguageClient,
    private readonly webviewPanel: vscode.WebviewPanel,
    private readonly extensionUri: vscode.Uri,
    private readonly documentUri: vscode.Uri,
  ) {
    super();

    this.onDidDisposeEmitter = new vscode.EventEmitter<void>();
    this.onDidChangeViewStateEmitter = this.register(
      new vscode.EventEmitter<vscode.WebviewPanelOnDidChangeViewStateEvent>(),
    );

    this.register(this.webviewPanel.webview.onDidReceiveMessage(this.handleDidReceiveMessage, this));

    if (!this.isDisposed) {
      this.webviewPanel.webview.html = this.createWebviewHtml();
    }

    this.registerMultiple(
      this.webviewPanel.onDidDispose(this.dispose, this),
      this.webviewPanel.onDidChangeViewState((e) => this.onDidChangeViewStateEmitter.fire(e)),
    );
  }

  public get onDidDispose(): vscode.Event<void> {
    return this.onDidDisposeEmitter.event;
  }

  public get onDidChangeViewState(): vscode.Event<vscode.WebviewPanelOnDidChangeViewStateEvent> {
    return this.onDidChangeViewStateEmitter.event;
  }

  public static create(
    extensionContext: ExtensionContext,
    context: IActionContext,
    azureMgr: IAzureUiManager,
    languageClient: LanguageClient,
    viewColumn: vscode.ViewColumn,
    extensionUri: vscode.Uri,
    documentUri: vscode.Uri,
  ): DeployPaneView {
    const visualizerTitle = `Deploy ${path.basename(documentUri.fsPath)}`;
    const webviewPanel = vscode.window.createWebviewPanel(DeployPaneView.viewType, visualizerTitle, viewColumn, {
      enableScripts: true,
      retainContextWhenHidden: true,
    });

    return new DeployPaneView(
      extensionContext,
      context,
      azureMgr,
      languageClient,
      webviewPanel,
      extensionUri,
      documentUri,
    );
  }

  public static revive(
    extensionContext: ExtensionContext,
    context: IActionContext,
    azureMgr: IAzureUiManager,
    languageClient: LanguageClient,
    webviewPanel: vscode.WebviewPanel,
    extensionUri: vscode.Uri,
    documentUri: vscode.Uri,
  ): DeployPaneView {
    return new DeployPaneView(
      extensionContext,
      context,
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

  public dispose(): void {
    super.dispose();

    this.webviewPanel.dispose();

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
      this.document = await vscode.workspace.openTextDocument(this.documentUri);
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
        this.render();
        return;
      }
      case "GET_STATE": {
        const deployPaneState: Record<string, DeployPaneState> =
          this.extensionContext.globalState.get(GlobalStateKeys.deployPaneStateKey) || {};
        const filteredState = deployPaneState[this.documentUri.toString()];

        await this.webviewPanel.webview.postMessage(createGetStateResultMessage(filteredState));
        return;
      }
      case "SAVE_STATE": {
        const deployPaneState: Record<string, DeployPaneState> =
          this.extensionContext.globalState.get(GlobalStateKeys.deployPaneStateKey) || {};
        deployPaneState[this.documentUri.toString()] = message.state;

        await this.extensionContext.globalState.update(GlobalStateKeys.deployPaneStateKey, deployPaneState);
        return;
      }
      case "PICK_PARAMS_FILE": {
        const parametersFileUri = await this.context.ui.showOpenDialog({
          canSelectMany: false,
          openLabel: "Select Parameters file",
          filters: { "Parameters files": ["json"] },
        });
        const parameterFile = await fse.readFile(parametersFileUri[0].fsPath, "utf-8");
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
      case "PUBLISH_TELEMETRY": {
        callWithTelemetryAndErrorHandlingSync(message.eventName, (telemetryActionContext) => {
          telemetryActionContext.errorHandling.suppressDisplay = true;
          telemetryActionContext.telemetry.properties = message.properties;
        });
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

  private createWebviewHtml() {
    const { cspSource } = this.webviewPanel.webview;
    const nonce = crypto.randomBytes(16).toString("hex");
    const scriptUri = this.webviewPanel.webview.asWebviewUri(
      vscode.Uri.joinPath(this.extensionUri, "out", "deploy-pane", "index.js"),
    );
    const codiconCssUri = this.webviewPanel.webview.asWebviewUri(
      vscode.Uri.joinPath(this.extensionUri, "out", "deploy-pane", "assets", "index.css"),
    );

    const armEndpoints = [
      Environment.AzureCloud,
      Environment.ChinaCloud,
      Environment.GermanCloud,
      Environment.USGovernment,
    ].map((env) => env.resourceManagerEndpointUrl);

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <!--
        Use a content security policy to only allow loading images from our extension directory,
        and only allow scripts that have a specific nonce.
        -->
        <meta http-equiv="Content-Security-Policy" content="default-src 'self' ${armEndpoints.join(" ")}; style-src ${cspSource} 'unsafe-inline'; img-src ${cspSource} data:; script-src 'nonce-${nonce}' vscode-webview-resource:; font-src data: ${cspSource};">
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
    const documentName = path.basename(this.documentUri.fsPath);

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
      </head>
      <body>
        <div class="vscode-body">${documentName} not found. It might be deleted or renamed.</div>
      </body>
      </html>`;
  }
}
