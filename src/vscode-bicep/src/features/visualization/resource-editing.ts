// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Uri } from "vscode";
import type { LanguageClient } from "vscode-languageclient/node";
import type { ReplayDirection, SourceStepReference } from "./source-edit-history";

import { TextDocument, workspace, WorkspaceEdit } from "vscode";
import { parseError } from "../../infrastructure/errors";
import { getLogger } from "../../infrastructure/logging";
import {
  prepareVisualResourceCreationRequestType,
  PrepareVisualResourceCreationResult,
  prepareVisualResourceReplayRequestType,
  VisualResourceReplayEdit,
  VisualResourceTypeReference,
} from "./protocol";
import { isResourceEditingEnabled } from "./resource-editing-setting";
import { SourceEditHistory, SourceEditHistoryConflict } from "./source-edit-history";

/** How a handler answers the webview request it is handling. */
export interface WebviewReply {
  postResponse(id: string, result: unknown): Promise<void>;
  postErrorResponse(id: string, error: unknown): Promise<void>;
  isDisposed(): boolean;
}
/** Whether a document changed since `requestedVersion`, including by being closed. */
function hasDocumentChanged(document: TextDocument, requestedVersion: number): boolean {
  return document.isClosed || document.version !== requestedVersion;
}

/** The designer's source steps sent with a graph update. Malformed entries are ignored. */
export function parseSourceSteps(value: unknown): SourceStepReference[] {
  if (!Array.isArray(value)) {
    return [];
  }

  return value.flatMap((step: unknown) => {
    if (typeof step !== "object" || step === null) {
      return [];
    }
    const { operationId, direction } = step as { operationId?: unknown; direction?: unknown };
    return typeof operationId === "string" && (direction === "undo" || direction === "redo")
      ? [{ operationId, direction }]
      : [];
  });
}

/**
 * The designer's source edits to one Bicep file: creating resources, and undoing and redoing those creations
 * with edits the language server prepares against the current document.
 */
export class ResourceEditing {
  private readonly sourceEditHistory = new SourceEditHistory();

  constructor(
    private readonly languageClient: LanguageClient,
    private readonly documentUri: Uri,
    private readonly reply: WebviewReply,
  ) {}

  /**
   * The source steps, of those sent with a graph request, that can be replayed exactly in the document. Null
   * when that could not be determined, so the webview offers none of them.
   */
  public async getReplayableSourceSteps(
    document: TextDocument,
    sourceSteps: unknown,
  ): Promise<SourceStepReference[] | null> {
    try {
      return (await this.prepareReplays(document, parseSourceSteps(sourceSteps)))
        .filter(({ edit }) => edit !== null)
        .map(({ step }) => step);
    } catch (error) {
      getLogger().error(`Designer undo/redo availability request failed: ${parseError(error).message}`);
      return null;
    }
  }
  public async createResource(id: string, params: unknown): Promise<void> {
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
      await this.reply.postErrorResponse(id, {
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
      await this.reply.postErrorResponse(id, {
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
      const result: PrepareVisualResourceCreationResult = await this.languageClient.sendRequest(
        prepareVisualResourceCreationRequestType,
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

      if (hasDocumentChanged(document, requestedVersion)) {
        await this.reply.postErrorResponse(id, {
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
        await this.reply.postErrorResponse(id, {
          version: 1,
          operationId: request.operationId,
          code: hasDocumentChanged(document, requestedVersion) ? "documentChanged" : "editRejected",
          message: "VS Code could not apply the generated resource declaration.",
          retryable: true,
        });
        return;
      }

      // The resource was created either way; a tracking failure only means the designer cannot undo it.
      let historyTrackingError: string | undefined;
      try {
        this.sourceEditHistory.recordInsertion(document, request.operationId, result.expectedNodeId, pendingInsertion);
      } catch (error) {
        if (!(error instanceof SourceEditHistoryConflict)) {
          throw error;
        }
        historyTrackingError = error.message;
        getLogger().error(`Visual resource creation history failed: ${error.message}`);
      }

      await this.reply.postResponse(id, {
        version: 1,
        operationId: result.operationId,
        expectedNodeId: result.expectedNodeId,
        unresolvedRequiredProperties: result.unresolvedRequiredProperties,
        historyTrackingError,
      });
    } catch (error) {
      getLogger().error(`Visual resource creation request failed: ${parseError(error).message}`);
      await this.reply.postErrorResponse(id, {
        version: 1,
        operationId: request.operationId,
        code: "generationFailed",
        message: "Failed to create the resource declaration.",
        retryable: true,
      });
    }
  }

  public async replaySourceStep(id: string, params: unknown): Promise<void> {
    const request = (params && typeof params === "object" ? params : {}) as {
      version?: number;
      operationId?: string;
      direction?: ReplayDirection;
    };
    if (
      request.version !== 1 ||
      typeof request.operationId !== "string" ||
      !request.operationId.trim() ||
      (request.direction !== "undo" && request.direction !== "redo")
    ) {
      await this.reply.postErrorResponse(id, {
        code: "invalidHistoryRequest",
        message: "The designer undo/redo request is invalid.",
        retryable: false,
      });
      return;
    }

    if (await this.rejectIfResourceEditingDisabled(id, request.operationId)) {
      return;
    }

    const step: SourceStepReference = { operationId: request.operationId, direction: request.direction };
    try {
      const document = await workspace.openTextDocument(this.documentUri);
      const requestedVersion = document.version;
      const [replay] = await this.prepareReplays(document, [step]);
      if (this.reply.isDisposed() || (await this.rejectIfResourceEditingDisabled(id, request.operationId))) {
        return;
      }

      // The designer only offers replays the language server confirmed with the last graph update, so this
      // only happens if the file changed since then.
      if (!replay?.edit) {
        await this.reply.postErrorResponse(id, {
          code: "replayUnavailable",
          message: "The Bicep file changed, so this designer action can no longer be undone or redone exactly.",
          retryable: false,
        });
        return;
      }
      if (hasDocumentChanged(document, requestedVersion)) {
        await this.reply.postErrorResponse(id, {
          code: "documentChanged",
          message: "The Bicep file changed before the designer action could be replayed.",
          retryable: true,
        });
        return;
      }

      const range = this.languageClient.protocol2CodeConverter.asRange(replay.edit.range);
      const pending = this.sourceEditHistory.prepareEdit(
        document,
        document.offsetAt(range.start),
        document.offsetAt(range.end),
        replay.edit.newText,
      );
      const edit = new WorkspaceEdit();
      edit.replace(document.uri, range, replay.edit.newText);

      if (!(await workspace.applyEdit(edit))) {
        await this.reply.postErrorResponse(id, {
          code: "editRejected",
          message: "VS Code could not apply the designer undo/redo edit.",
          retryable: true,
        });
        return;
      }

      this.sourceEditHistory.commitReplay(document, request.operationId, request.direction, pending);
      await this.reply.postResponse(id, { version: 1, operationId: request.operationId, direction: request.direction });
    } catch (error) {
      if (error instanceof SourceEditHistoryConflict) {
        await this.reply.postErrorResponse(id, { code: "replayUnavailable", message: error.message, retryable: false });
        return;
      }

      getLogger().error(`Designer source undo/redo failed: ${parseError(error).message}`);
      await this.reply.postErrorResponse(id, {
        code: "replayFailed",
        message: "Failed to undo or redo the designer source edit.",
        retryable: true,
      });
    }
  }

  /** Ask the language server for the edit that replays each step exactly against the document, or null. */
  private async prepareReplays(
    document: TextDocument,
    steps: readonly SourceStepReference[],
  ): Promise<{ step: SourceStepReference; edit: VisualResourceReplayEdit["edit"] }[]> {
    const replays = this.sourceEditHistory.getReplayQueries(steps);
    const result =
      replays.length === 0
        ? { replays: [] }
        : await this.languageClient.sendRequest(prepareVisualResourceReplayRequestType, {
            textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
            replays,
          });

    // The language server answers each query in order.
    return steps.map((step) => {
      const index = replays.findIndex(
        (replay) => replay.operationId === step.operationId && replay.direction === step.direction,
      );
      const edit = result.replays[index];
      return { step, edit: edit?.operationId === step.operationId ? edit.edit : null };
    });
  }
  private async rejectIfResourceEditingDisabled(id: string, operationId: string): Promise<boolean> {
    if (isResourceEditingEnabled()) {
      return false;
    }

    await this.reply.postErrorResponse(id, {
      version: 1,
      operationId,
      code: "editingDisabled",
      message: "Resource editing is disabled. Enable the experimental resource-editing setting to change resources.",
      retryable: false,
    });
    return true;
  }
}
