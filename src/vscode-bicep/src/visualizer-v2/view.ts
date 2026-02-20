// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import crypto from "crypto";
import path from "path";
import { parseError } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { deploymentGraphRequestType } from "../language";
import { Disposable } from "../utils/disposable";
import { getLogger } from "../utils/logger";
import { debounce } from "../utils/time";

export class VisualDesignerView extends Disposable {
  public static viewType = "bicep.visualDesigner";

  private readonly onDidDisposeEmitter: vscode.EventEmitter<void>;
  private readonly onDidChangeViewStateEmitter: vscode.EventEmitter<vscode.WebviewPanelOnDidChangeViewStateEvent>;

  private readyToRender = false;

  private constructor(
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
    languageClient: LanguageClient,
    viewColumn: vscode.ViewColumn,
    extensionUri: vscode.Uri,
    documentUri: vscode.Uri,
  ): VisualDesignerView {
    const visualizerTitle = `Visualize ${path.basename(documentUri.fsPath)}`;
    const webviewPanel = vscode.window.createWebviewPanel(VisualDesignerView.viewType, visualizerTitle, viewColumn, {
      enableScripts: true,
      retainContextWhenHidden: true,
    });

    return new VisualDesignerView(languageClient, webviewPanel, extensionUri, documentUri);
  }

  public static revive(
    languageClient: LanguageClient,
    webviewPanel: vscode.WebviewPanel,
    extensionUri: vscode.Uri,
    documentUri: vscode.Uri,
  ): VisualDesignerView {
    return new VisualDesignerView(languageClient, webviewPanel, extensionUri, documentUri);
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

    let document: vscode.TextDocument;
    try {
      document = await vscode.workspace.openTextDocument(this.documentUri);
    } catch {
      this.webviewPanel.webview.html = this.createDocumentNotFoundHtml();
      return;
    }

    if (this.isDisposed) {
      return;
    }

    const deploymentGraph = await this.languageClient.sendRequest(deploymentGraphRequestType, {
      textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
    });

    if (this.isDisposed) {
      return;
    }

    try {
      // Send as a notification using the messaging library's format
      await this.webviewPanel.webview.postMessage({
        method: "deploymentGraph",
        params: {
          documentPath: this.documentUri.fsPath,
          deploymentGraph,
        },
      });
    } catch (error) {
      // Race condition: the webview was closed before receiving the message,
      // which causes "Unknown webview handle" error.
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private handleDidReceiveMessage(message: unknown): void {
    if (!message || typeof message !== "object") {
      return;
    }

    // Handle notification messages (method-based, no id)
    if ("method" in message && !("id" in message)) {
      const notification = message as { method: string; params?: unknown };

      switch (notification.method) {
        case "ready":
          getLogger().debug(`Visual Designer for ${this.documentUri.fsPath} is ready.`);
          this.readyToRender = true;
          this.render();
          return;

        case "revealFileRange": {
          const payload = notification.params as { filePath: string; range: vscode.Range };
          this.revealFileRange(payload.filePath, payload.range);
          return;
        }
      }
    }

    // Handle request messages (have id — need response)
    if ("id" in message && "method" in message) {
      const request = message as { id: string; method: string; params?: unknown };
      // Future: handle request/response patterns if needed
      // For now, no requests are expected from the webview
      getLogger().warn(`Unhandled request method: ${request.method}`);
    }
  }

  private revealFileRange(filePath: string, range: vscode.Range) {
    for (const visibleEditor of vscode.window.visibleTextEditors) {
      if (visibleEditor.document.uri.fsPath === filePath) {
        vscode.window.showTextDocument(visibleEditor.document, visibleEditor.viewColumn).then(
          (editor) => this.revealEditorRange(editor, range),
          (err) =>
            vscode.window.showErrorMessage(`Could not reveal file range in "${filePath}": ${parseError(err).message}`),
        );
        return;
      }
    }

    vscode.workspace
      .openTextDocument(filePath)
      .then(vscode.window.showTextDocument)
      .then(
        (editor) => this.revealEditorRange(editor, range),
        (err) => vscode.window.showErrorMessage(`Could not open "${filePath}": ${parseError(err).message}`),
      );
  }

  private revealEditorRange(editor: vscode.TextEditor, range: vscode.Range) {
    const cursorPosition = editor.selection.active.with(range.start.line, range.start.character);
    editor.selection = new vscode.Selection(cursorPosition, cursorPosition);
    editor.revealRange(range, vscode.TextEditorRevealType.InCenter);
  }

  private createWebviewHtml() {
    const { cspSource } = this.webviewPanel.webview;
    const nonce = crypto.randomBytes(16).toString("hex");

    const scriptUri = this.webviewPanel.webview.asWebviewUri(
      vscode.Uri.joinPath(this.extensionUri, "out", "visual-designer", "index.js"),
    );
    const cssUri = this.webviewPanel.webview.asWebviewUri(
      vscode.Uri.joinPath(this.extensionUri, "out", "visual-designer", "assets", "index.css"),
    );

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src ${cspSource} 'unsafe-inline'; img-src ${cspSource} data:; script-src 'nonce-${nonce}' vscode-webview-resource:; font-src data: ${cspSource};">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link rel="stylesheet" nonce="${nonce}" href="${cssUri}">
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
