// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { ReplayDirection } from "./source-edit-history";

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
  WorkspaceEdit,
} from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { parseError } from "../../infrastructure/errors";
import { Disposable } from "../../infrastructure/lifecycle";
import { getLogger } from "../../infrastructure/logging";
import { debounce } from "../../infrastructure/timing";
import { getVisualizerMotionPolicy } from "./motion-policy";
import {
  createResourceDeclarationInsertionRequestType,
  ResourceDeclarationInsertion,
  visualGraphLayoutRequestType,
  VisualGraphLayoutResult,
  visualGraphNodeSourceRequestType,
  VisualGraphRendered,
  visualGraphUpdateRequestType,
  VisualGraphUpdateResult,
  VisualResourceTypeCatalogItem,
  visualResourceTypeNamespacesRequestType,
  VisualResourceTypeReference,
  visualResourceTypesRequestType,
  visualResourceTypeVersionsRequestType,
} from "./protocol";
import { getApplyEditFailureCode, hasDocumentChanged } from "./resource-creation";
import { isResourceEditingEnabled } from "./resource-editing-setting";
import { buildResourceTypeCatalog } from "./resource-palette";
import { SourceEditHistory, SourceEditHistoryConflict } from "./source-edit-history";

export class BicepVisualizerView extends Disposable {
  public static viewType = "bicep.visualizer";

  private readonly onDidDisposeEmitter: EventEmitter<void>;
  private readonly onDidChangeViewStateEmitter: EventEmitter<WebviewPanelOnDidChangeViewStateEvent>;
  private readonly ready: Promise<void>;
  private resolveReady!: () => void;

  private readyToRender = false;
  private readonly sourceEditHistory = new SourceEditHistory();

  private constructor(
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

  public notifyMotionPolicyDidChange(): void {
    if (this.isDisposed) {
      return;
    }

    void this.webviewPanel.webview
      .postMessage({
        method: "motionPolicy/didChange",
        params: getVisualizerMotionPolicy(),
      })
      .then(undefined, (error: unknown) => getLogger().debug(parseError(error).message));
  }

  public notifyResourceCreationEnablementDidChange(): void {
    if (this.isDisposed) {
      return;
    }

    void this.webviewPanel.webview
      .postMessage({
        method: "resourceCreation/enablementDidChange",
        params: isResourceEditingEnabled(),
      })
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

  // Do "fire and forget" since there's no need to wait on rendering.
  public render = debounce(() => this.doRender());

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
        method: "documentDidChange",
        params: { documentUri: this.documentUri.fsPath },
      });
    } catch (error) {
      // Race condition: the webview was closed before receiving the message.
      getLogger().debug((error as Error).message ?? error);
    }
  }

  private async handleGetGraphUpdate(id: string, params: unknown): Promise<void> {
    const current = (params as { current?: VisualGraphRendered | null })?.current ?? null;
    let result: VisualGraphUpdateResult = { patches: [], targetScope: null };

    try {
      const document = await workspace.openTextDocument(this.documentUri);

      if (this.isDisposed) {
        return;
      }

      result = await this.languageClient.sendRequest(visualGraphUpdateRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
        current,
      });
    } catch (error) {
      // Keep the webview responsive: an empty delta means "nothing changed", so it keeps what it has.
      getLogger().error(`Visual graph update request failed: ${parseError(error).message}`);
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

  private async handleGetGraphLayout(id: string, params: unknown): Promise<void> {
    const current = (params as { current?: VisualGraphRendered })?.current;
    let result: VisualGraphLayoutResult = { status: "layoutFailed", patches: [] };

    if (!current) {
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
        current,
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

  private async handleCreateResource(id: string, params: unknown): Promise<void> {
    const request = (params && typeof params === "object" ? params : {}) as {
      version?: number;
      operationId?: string;
      resourceType?: VisualResourceTypeReference;
    };

    if (
      request.version !== 1 ||
      typeof request.operationId !== "string" ||
      !request.operationId.trim() ||
      typeof request.resourceType?.fullyQualifiedType !== "string" ||
      !request.resourceType.fullyQualifiedType.trim() ||
      typeof request.resourceType.apiVersion !== "string" ||
      !request.resourceType.apiVersion.trim()
    ) {
      await this.postErrorResponse(id, {
        version: 1,
        operationId: request.operationId,
        code: request.version === 1 ? "invalidResourceType" : "unsupportedContract",
        message:
          request.version === 1
            ? "The resource type selection is invalid."
            : "The resource creation contract version is not supported.",
        retryable: false,
      });
      return;
    }

    if (await this.rejectIfResourceEditingDisabled(id, request.operationId)) {
      return;
    }

    if (this.sourceEditHistory.hasOperation(request.operationId)) {
      await this.postErrorResponse(id, {
        version: 1,
        operationId: request.operationId,
        code: "duplicateOperation",
        message: "This resource creation request has already been applied.",
        retryable: false,
      });
      return;
    }

    try {
      const document = await workspace.openTextDocument(this.documentUri);
      const requestedVersion = document.version;
      const result: ResourceDeclarationInsertion = await this.languageClient.sendRequest(
        createResourceDeclarationInsertionRequestType,
        {
          textDocument: this.languageClient.code2ProtocolConverter.asVersionedTextDocumentIdentifier(document),
          operationId: request.operationId,
          resourceType: request.resourceType,
        },
      );
      const changes = result.edit.documentChanges;
      if (
        changes?.length !== 1 ||
        !("textDocument" in changes[0]) ||
        changes[0].textDocument.version !== requestedVersion
      ) {
        throw new Error("Resource creation returned an unsupported or stale workspace edit.");
      }
      const edit = await this.languageClient.protocol2CodeConverter.asWorkspaceEdit(result.edit);

      if (await this.rejectIfResourceEditingDisabled(id, request.operationId)) {
        return;
      }

      if (hasDocumentChanged(requestedVersion, document.version, document.isClosed)) {
        await this.postErrorResponse(id, {
          version: 1,
          operationId: request.operationId,
          code: "documentChanged",
          message: "The Bicep file changed before the generated resource declaration could be applied.",
          retryable: true,
        });
        return;
      }

      const pendingInsertion = this.sourceEditHistory.prepareInsertion(document, edit);
      const applied = await workspace.applyEdit(edit);

      if (!applied) {
        await this.postErrorResponse(id, {
          version: 1,
          operationId: request.operationId,
          code: getApplyEditFailureCode(requestedVersion, document.version, document.isClosed),
          message: "VS Code could not apply the generated resource declaration.",
          retryable: true,
        });
        return;
      }

      // The resource was created either way; a tracking failure only means the designer cannot undo it.
      let historyEpoch: number;
      let historyTrackingError: string | undefined;
      try {
        historyEpoch = this.sourceEditHistory.recordInsertion(document, request.operationId, pendingInsertion);
      } catch (error) {
        if (!(error instanceof SourceEditHistoryConflict)) {
          throw error;
        }
        historyEpoch = this.sourceEditHistory.epoch;
        historyTrackingError = error.message;
        getLogger().error(`Visual resource creation history failed: ${error.message}`);
      }

      await this.postResponse(id, {
        version: 1,
        operationId: result.operationId,
        expectedNodeId: result.expectedNodeId,
        symbolicName: result.symbolicName,
        unresolvedRequiredProperties: result.unresolvedRequiredProperties,
        historyEpoch,
        historyTrackingError,
      });
    } catch (error) {
      getLogger().error(`Visual resource creation request failed: ${parseError(error).message}`);
      await this.postErrorResponse(id, {
        version: 1,
        operationId: request.operationId,
        code: "generationFailed",
        message: "Failed to create the resource declaration.",
        retryable: true,
      });
    }
  }

  private async handleReplaySourceEdit(id: string, params: unknown): Promise<void> {
    const request = (params && typeof params === "object" ? params : {}) as {
      version?: number;
      operationId?: string;
      direction?: ReplayDirection;
      historyEpoch?: number;
    };
    if (
      request.version !== 1 ||
      typeof request.operationId !== "string" ||
      !request.operationId.trim() ||
      (request.direction !== "undo" && request.direction !== "redo") ||
      typeof request.historyEpoch !== "number" ||
      !Number.isSafeInteger(request.historyEpoch) ||
      request.historyEpoch < 0
    ) {
      await this.postErrorResponse(id, {
        code: "invalidHistoryRequest",
        message: "The designer undo/redo request is invalid.",
        retryable: false,
      });
      return;
    }

    if (await this.rejectIfResourceEditingDisabled(id, request.operationId)) {
      return;
    }

    if (request.historyEpoch !== this.sourceEditHistory.epoch) {
      await this.postErrorResponse(id, {
        code: "historyConflict",
        message: "The source edit history changed. Refresh the visualizer before undoing this edit.",
        historyEpoch: this.sourceEditHistory.epoch,
        retryable: false,
      });
      return;
    }

    try {
      const document = await workspace.openTextDocument(this.documentUri);
      if (this.isDisposed) {
        return;
      }

      const plan = this.sourceEditHistory.planReplay(document, request.operationId, request.direction);
      const edit = new WorkspaceEdit();
      edit.replace(
        document.uri,
        new Range(document.positionAt(plan.startOffset), document.positionAt(plan.endOffset)),
        plan.newText,
      );

      if (await this.rejectIfResourceEditingDisabled(id, request.operationId)) {
        return;
      }
      if (document.version !== plan.documentVersionBeforeReplay) {
        this.sourceEditHistory.invalidate();
        throw new SourceEditHistoryConflict("The Bicep file changed before the designer action could be replayed.");
      }

      if (!(await workspace.applyEdit(edit))) {
        await this.postErrorResponse(id, {
          code: "editRejected",
          message: "VS Code could not apply the designer undo/redo edit.",
          retryable: true,
        });
        return;
      }

      const historyEpoch = this.sourceEditHistory.commitReplay(document, plan);
      await this.postResponse(id, {
        version: 1,
        operationId: request.operationId,
        direction: request.direction,
        historyEpoch,
      });
    } catch (error) {
      if (error instanceof SourceEditHistoryConflict) {
        await this.postErrorResponse(id, {
          code: "historyConflict",
          message: error.message,
          historyEpoch: this.sourceEditHistory.epoch,
          retryable: false,
        });
        return;
      }

      getLogger().error(`Designer source undo/redo failed: ${parseError(error).message}`);
      await this.postErrorResponse(id, {
        code: "replayFailed",
        message: "Failed to undo or redo the designer source edit.",
        retryable: true,
      });
    }
  }

  private async rejectIfResourceEditingDisabled(id: string, operationId: string): Promise<boolean> {
    if (isResourceEditingEnabled()) {
      return false;
    }

    await this.postErrorResponse(id, {
      version: 1,
      operationId,
      code: "editingDisabled",
      message: "Resource editing is disabled. Enable the experimental resource-editing setting to change resources.",
      retryable: false,
    });
    return true;
  }

  private async handleGetResourceTypeNamespaces(id: string): Promise<void> {
    try {
      const document = await workspace.openTextDocument(this.documentUri);
      const result = await this.languageClient.sendRequest(visualResourceTypeNamespacesRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
      });

      await this.postResponse(id, result);
    } catch (error) {
      getLogger().error(`Resource type namespace request failed: ${parseError(error).message}`);
      await this.postErrorResponse(id, { message: "Failed to load resource provider namespaces." });
    }
  }

  private async handleGetResourceTypeCatalog(id: string, params: unknown): Promise<void> {
    const request = params as { providerNamespace?: unknown; query?: unknown; loadAll?: unknown };
    const providerNamespace =
      typeof request.providerNamespace === "string" && request.providerNamespace.trim()
        ? request.providerNamespace.trim()
        : undefined;
    const query = typeof request.query === "string" && request.query.trim() ? request.query.trim() : undefined;
    const loadAll = request.loadAll === true;

    if (!providerNamespace && !query && !loadAll) {
      await this.postErrorResponse(id, { message: "A provider namespace or search query is required." });
      return;
    }

    try {
      const document = await workspace.openTextDocument(this.documentUri);
      const items: VisualResourceTypeCatalogItem[] = [];
      let catalogId: string | undefined;
      let continuationToken: string | undefined;

      do {
        const response = await this.languageClient.sendRequest(visualResourceTypesRequestType, {
          textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
          providerNamespace,
          query,
          pageSize: 200,
          continuationToken,
        });
        catalogId ??= response.catalogId;
        if (catalogId !== response.catalogId) {
          throw new Error("The resource type catalog changed while it was being loaded.");
        }
        items.push(...response.items);
        continuationToken = response.continuationToken;
      } while (continuationToken);

      await this.postResponse(id, { catalogId, groups: buildResourceTypeCatalog(items) });
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
        case "ready":
          getLogger().debug(`Visualizer for ${this.documentUri.fsPath} is ready.`);
          this.readyToRender = true;
          this.resolveReady();
          this.render();
          return;

        case "revealNodeSource": {
          const payload = notification.params as { nodeId: string };
          void this.handleRevealNodeSource(payload.nodeId);
          return;
        }

        case "showProblemsPanel":
          commands.executeCommand("workbench.actions.view.problems");
          return;
      }
    }

    // Handle request messages (have id — need response)
    if ("id" in message && "method" in message) {
      const request = message as { id: string; method: string; params?: unknown };

      switch (request.method) {
        case "getGraphUpdate":
          void this.handleGetGraphUpdate(request.id, request.params);
          return;

        case "getGraphLayout":
          void this.handleGetGraphLayout(request.id, request.params);
          return;

        case "resources/create":
          void this.handleCreateResource(request.id, request.params);
          return;

        case "undoHistory/replaySourceEdit":
          void this.handleReplaySourceEdit(request.id, request.params);
          return;

        case "resourceTypeCatalog/load":
          void this.handleGetResourceTypeCatalog(request.id, request.params);
          return;

        case "resourceTypeCatalog/namespaces":
          void this.handleGetResourceTypeNamespaces(request.id);
          return;

        case "resourceTypeCatalog/versions":
          void this.handleGetResourceTypeVersions(request.id, request.params);
          return;

        case "motionPolicy/get":
          void this.postResponse(request.id, getVisualizerMotionPolicy());
          return;

        case "resourceCreation/isEnabled":
          void this.postResponse(request.id, isResourceEditingEnabled());
          return;
      }

      getLogger().warn(`Unhandled request method: ${request.method}`);
    }
  }

  private async handleRevealNodeSource(nodeId: string): Promise<void> {
    try {
      const document = await workspace.openTextDocument(this.documentUri);

      if (this.isDisposed) {
        return;
      }

      const result = await this.languageClient.sendRequest(visualGraphNodeSourceRequestType, {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
        nodeId,
      });

      if (this.isDisposed || !result.found || !result.filePath || !result.range) {
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
