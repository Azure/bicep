// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { SourceStepReference } from "./source-edit-history";

import crypto from "crypto";
import path from "path";
import {
  commands,
  Event,
  EventEmitter,
  Range,
  Selection,
  TextEditor,
  TextEditorRevealType,
  Uri,
  ViewColumn,
  WebviewPanel,
  WebviewPanelOnDidChangeViewStateEvent,
  window,
  workspace,
} from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { parseError } from "../../infrastructure/errors";
import { Disposable } from "../../infrastructure/lifecycle";
import { getLogger } from "../../infrastructure/logging";
import { getVisualizerMotionPolicy } from "./motion-policy";
import {
  visualGraphLayoutRequestType,
  VisualGraphLayoutResult,
  visualGraphNodeSourceRequestType,
  visualGraphRequestType,
  VisualGraphResult,
  VisualizerSettings,
  visualResourceTypesRequestType,
  visualResourceTypeVersionsRequestType,
} from "./protocol";
import { ResourceEditing } from "./resource-editing";
import { isResourceEditingEnabled } from "./resource-editing-setting";

export class BicepVisualizerView extends Disposable {
  public static viewType = "bicep.visualizer";

  private readonly onDidDisposeEmitter: EventEmitter<void>;
  private readonly onDidChangeViewStateEmitter: EventEmitter<WebviewPanelOnDidChangeViewStateEvent>;
  private readonly ready: Promise<void>;
  private resolveReady!: () => void;

  private readyToRender = false;
  private readonly resourceEditing: ResourceEditing;

  private constructor(
    private readonly languageClient: LanguageClient,
    private readonly webviewPanel: WebviewPanel,
    private readonly extensionUri: Uri,
    private readonly documentUri: Uri,
  ) {
    super();

    this.resourceEditing = new ResourceEditing(languageClient, documentUri, {
      postResponse: (id, result) => this.postResponse(id, result),
      postErrorResponse: (id, error) => this.postErrorResponse(id, error),
      isDisposed: () => this.isDisposed,
    });
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
    languageClient: LanguageClient,
    viewColumn: ViewColumn,
    extensionUri: Uri,
    documentUri: Uri,
  ): BicepVisualizerView {
    const visualizerTitle = `Visualize ${path.basename(documentUri.fsPath)}`;
    const webviewPanel = window.createWebviewPanel(BicepVisualizerView.viewType, visualizerTitle, viewColumn, {
      enableScripts: true,
      retainContextWhenHidden: true,
    });

    return new BicepVisualizerView(languageClient, webviewPanel, extensionUri, documentUri);
  }

  public static revive(
    languageClient: LanguageClient,
    webviewPanel: WebviewPanel,
    extensionUri: Uri,
    documentUri: Uri,
  ): BicepVisualizerView {
    return new BicepVisualizerView(languageClient, webviewPanel, extensionUri, documentUri);
  }

  public reveal(viewColumn?: ViewColumn): void {
    this.webviewPanel.reveal(viewColumn);
  }

  public async waitUntilReady(): Promise<void> {
    await this.ready;
    if (this.isDisposed) {
      throw new Error("The visualizer was disposed before it became ready.");
    }
  }

  /** Send the settings the webview depends on. Sent when it becomes ready and whenever one of them changes. */
  public notifySettingsDidChange(): void {
    if (this.isDisposed || !this.readyToRender) {
      return;
    }

    const settings: VisualizerSettings = {
      motionPolicy: getVisualizerMotionPolicy(),
      isResourceEditingEnabled: isResourceEditingEnabled(),
    };
    void this.webviewPanel.webview
      .postMessage({ method: "settings/didChange", params: settings })
      .then(undefined, (error: unknown) => getLogger().debug(parseError(error).message));
  }

  public dispose(): void {
    super.dispose();

    this.webviewPanel.dispose();
    this.resolveReady();

    // Final cleanup.
    this.onDidDisposeEmitter.fire();
    this.onDidDisposeEmitter.dispose();
  }

  /**
   * Tell the webview the document may have changed. Not debounced: the webview withdraws anything derived from the
   * previous content, such as which designer steps can be replayed, as soon as it hears, and paces its own graph updates.
   */
  public render(): void {
    void this.doRender();
  }

  private async doRender() {
    if (this.isDisposed || !this.readyToRender) {
      return;
    }

    try {
      await workspace.openTextDocument(this.documentUri);
    } catch {
      this.webviewPanel.webview.html = this.createDocumentNotFoundHtml();
      return;
    }

    if (this.isDisposed) {
      return;
    }

    await this.notifyDocumentDidChange();
  }

  private async notifyDocumentDidChange(): Promise<void> {
    try {
      await this.webviewPanel.webview.postMessage({
        method: "document/didChange",
        params: { documentUri: this.documentUri.fsPath },
      });
    } catch (error) {
      // Race condition: the webview was closed before receiving the message.
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private async handleGetGraph(id: string, params: unknown): Promise<void> {
    let result: VisualGraphResult = { graph: null, targetScope: null, errorCount: 0 };
    let replayableSourceSteps: SourceStepReference[] | null = null;

    try {
      const document = await workspace.openTextDocument(this.documentUri);

      if (this.isDisposed) {
        return;
      }

      result = await this.languageClient.sendRequest(visualGraphRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
      });

      // Computed with every graph update, so the designer only enables Undo and Redo for creations that can be
      // replayed exactly in the document it is showing.
      replayableSourceSteps = await this.resourceEditing.getReplayableSourceSteps(
        document,
        (params as { sourceSteps?: unknown })?.sourceSteps,
      );
    } catch (error) {
      // Keep the webview responsive: a null graph means "unknown", so it keeps what it has.
      getLogger().error(`Visual graph request failed: ${parseError(error).message}`);
    }

    if (this.isDisposed) {
      return;
    }

    try {
      await this.webviewPanel.webview.postMessage({ id, result: { ...result, replayableSourceSteps } });
    } catch (error) {
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private async handleLayoutGraph(id: string, params: unknown): Promise<void> {
    const graph = (params as { graph?: unknown })?.graph;
    let result: VisualGraphLayoutResult = { status: "layoutFailed", positions: [], bounds: null };

    if (!graph) {
      await this.webviewPanel.webview.postMessage({ id, result });
      return;
    }

    try {
      const document = await workspace.openTextDocument(this.documentUri);

      if (this.isDisposed) {
        return;
      }

      result = await this.languageClient.sendRequest(visualGraphLayoutRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
        graph,
      });
    } catch (error) {
      getLogger().error(`Visual graph layout request failed: ${parseError(error).message}`);
    }

    if (this.isDisposed) {
      return;
    }

    try {
      await this.webviewPanel.webview.postMessage({ id, result });
    } catch (error) {
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private async handleListResourceTypes(id: string, params: unknown): Promise<void> {
    const knownCatalogId = (params as { knownCatalogId?: unknown } | undefined)?.knownCatalogId;

    try {
      const document = await workspace.openTextDocument(this.documentUri);
      const { catalogId, resourceTypes } = await this.languageClient.sendRequest(visualResourceTypesRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
        knownCatalogId: typeof knownCatalogId === "string" ? knownCatalogId : undefined,
      });

      await this.postResponse(id, {
        catalogId,
        resourceTypes:
          resourceTypes?.map(({ fullyQualifiedType, apiVersion }) => ({ fullyQualifiedType, apiVersion })) ?? null,
      });
    } catch (error) {
      getLogger().error(`Resource type catalog request failed: ${parseError(error).message}`);
      await this.postErrorResponse(id, { message: "Failed to load resource types for this Bicep file." });
    }
  }
  private async handleGetResourceTypeVersions(id: string, params: unknown): Promise<void> {
    if (
      typeof params !== "object" ||
      params === null ||
      !("fullyQualifiedType" in params) ||
      typeof params.fullyQualifiedType !== "string" ||
      !params.fullyQualifiedType.trim()
    ) {
      await this.postErrorResponse(id, { message: "A resource type is required." });
      return;
    }

    try {
      const document = await workspace.openTextDocument(this.documentUri);
      const result = await this.languageClient.sendRequest(visualResourceTypeVersionsRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
        fullyQualifiedType: params.fullyQualifiedType.trim(),
      });

      await this.postResponse(id, result);
    } catch (error) {
      getLogger().error(`Resource type API versions request failed: ${parseError(error).message}`);
      await this.postErrorResponse(id, { message: "Failed to load API versions for this resource type." });
    }
  }

  private async postResponse(id: string, result: unknown): Promise<void> {
    if (this.isDisposed) {
      return;
    }

    try {
      await this.webviewPanel.webview.postMessage({ id, result });
    } catch (error) {
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private async postErrorResponse(id: string, error: unknown): Promise<void> {
    if (this.isDisposed) {
      return;
    }

    try {
      await this.webviewPanel.webview.postMessage({ id, error });
    } catch (postError) {
      getLogger().debug((postError as Error).message ?? postError);
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
        case "webview/ready":
          getLogger().debug(`Visualizer for ${this.documentUri.fsPath} is ready.`);
          this.readyToRender = true;
          this.resolveReady();
          this.notifySettingsDidChange();
          this.render();
          return;

        case "document/revealNode": {
          const payload = notification.params as { nodeId: string };
          void this.handleRevealNode(payload.nodeId);
          return;
        }

        case "problems/show":
          commands.executeCommand("workbench.actions.view.problems");
          return;
      }
    }

    // Handle request messages (have id — need response)
    if ("id" in message && "method" in message) {
      const request = message as { id: string; method: string; params?: unknown };

      switch (request.method) {
        case "graph/get":
          void this.handleGetGraph(request.id, request.params);
          return;

        case "graph/layout":
          void this.handleLayoutGraph(request.id, request.params);
          return;

        case "resources/create":
          void this.resourceEditing.createResource(request.id, request.params);
          return;

        case "history/replaySourceStep":
          void this.resourceEditing.replaySourceStep(request.id, request.params);
          return;

        case "resourceTypes/list":
          void this.handleListResourceTypes(request.id, request.params);
          return;

        case "resourceTypes/versions":
          void this.handleGetResourceTypeVersions(request.id, request.params);
          return;
      }

      getLogger().warn(`Unhandled request method: ${request.method}`);
    }
  }

  private async handleRevealNode(nodeId: string): Promise<void> {
    try {
      const document = await workspace.openTextDocument(this.documentUri);

      if (this.isDisposed) {
        return;
      }

      const result = await this.languageClient.sendRequest(visualGraphNodeSourceRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
        nodeId,
      });

      if (this.isDisposed || !result.filePath || !result.range) {
        return;
      }

      this.revealFileRange(result.filePath, this.languageClient.protocol2CodeConverter.asRange(result.range));
    } catch (error) {
      getLogger().error(`Visual graph node source request failed: ${parseError(error).message}`);
    }
  }

  private revealFileRange(filePath: string, range: Range) {
    for (const visibleEditor of window.visibleTextEditors) {
      if (visibleEditor.document.uri.fsPath === filePath) {
        window.showTextDocument(visibleEditor.document, visibleEditor.viewColumn).then(
          (editor) => this.revealEditorRange(editor, range),
          (err) => getLogger().error(`Could not reveal file range in "${filePath}": ${parseError(err).message}`),
        );
        return;
      }
    }

    const targetColumn = this.getTextEditorViewColumn() ?? ViewColumn.Beside;

    workspace
      .openTextDocument(filePath)
      .then((doc) => window.showTextDocument(doc, targetColumn))
      .then(
        (editor) => this.revealEditorRange(editor, range),
        (err) => getLogger().error(`Could not open "${filePath}": ${parseError(err).message}`),
      );
  }

  private getTextEditorViewColumn(): ViewColumn | undefined {
    const webviewColumn = this.webviewPanel.viewColumn;

    for (const editor of window.visibleTextEditors) {
      if (editor.viewColumn !== undefined && editor.viewColumn !== webviewColumn) {
        return editor.viewColumn;
      }
    }

    return undefined;
  }

  private revealEditorRange(editor: TextEditor, range: Range) {
    const cursorPosition = editor.selection.active.with(range.start.line, range.start.character);
    editor.selection = new Selection(cursorPosition, cursorPosition);
    editor.revealRange(range, TextEditorRevealType.InCenter);
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
      Uri.joinPath(this.extensionUri, "out", "visual-designer", "index.js"),
    );
    const cssUri = this.webviewPanel.webview.asWebviewUri(
      Uri.joinPath(this.extensionUri, "out", "visual-designer", "assets", "index.css"),
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
