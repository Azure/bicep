// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { TextDocument, WorkspaceEdit } from "vscode";
import type { VisualResourceReplayDirection, VisualResourceReplayQuery } from "./protocol";

export type ReplayDirection = VisualResourceReplayDirection;

/** A designer edit was applied, but the document did not end up as expected, so it cannot be tracked. */
export class SourceEditHistoryConflict extends Error {
  constructor(message: string) {
    super(message);
    this.name = "SourceEditHistoryConflict";
  }
}

/** A designer edit about to be applied. `recordInsertion` and `commitReplay` check the result against it. */
export interface PendingSourceEdit {
  startOffset: number;
  endOffset: number;
  text: string;
  documentTextBefore: string;
  documentVersionBefore: number;
}

/** One resource creation in the designer's undo history, identified by the operation that made it. */
export interface SourceStepReference {
  operationId: string;
  direction: ReplayDirection;
}

interface TrackedCreation {
  nodeId: string;
  /** The text the creation last inserted, as the document actually contains it after any line-ending normalization. */
  insertedText: string;
}

function normalizeLineEndings(text: string): string {
  return text.replace(/\r\n?|\n/g, "\n");
}

/**
 * The resource creations the designer made in this document, kept so the language server can work out whether
 * each can still be undone or redone exactly.
 *
 * Nothing here tracks positions or other edits: the language server finds the declaration in the current
 * document every time, so edits made elsewhere in the file never affect whether a creation can be replayed.
 */
export class SourceEditHistory {
  private readonly creationsByOperationId = new Map<string, TrackedCreation>();

  hasOperation(operationId: string): boolean {
    return this.creationsByOperationId.has(operationId);
  }

  /** The language server queries for the given steps, skipping operations this history does not know. */
  getReplayQueries(steps: readonly SourceStepReference[]): VisualResourceReplayQuery[] {
    return steps.flatMap(({ operationId, direction }) => {
      const creation = this.creationsByOperationId.get(operationId);
      return creation ? [{ operationId, direction, nodeId: creation.nodeId, insertedText: creation.insertedText }] : [];
    });
  }

  prepareInsertion(document: TextDocument, edit: WorkspaceEdit): PendingSourceEdit {
    const entries = edit.entries();
    const [entry] = entries;
    if (
      entries.length !== 1 ||
      !entry ||
      entry[0].toString() !== document.uri.toString() ||
      entry[1].length !== 1 ||
      !entry[1][0]?.range.isEmpty ||
      !entry[1][0].newText
    ) {
      throw new Error("Resource creation must insert text into the opened Bicep file only.");
    }

    const offset = document.offsetAt(entry[1][0].range.start);
    return this.prepareEdit(document, offset, offset, entry[1][0].newText);
  }

  /** Verify an applied creation and start tracking it. */
  recordInsertion(document: TextDocument, operationId: string, nodeId: string, pending: PendingSourceEdit): void {
    const insertedText = getAppliedText(document, pending);
    if (!insertedText) {
      throw new SourceEditHistoryConflict(
        "The resource was created, but the Bicep file changed unexpectedly. Use the editor's Undo command.",
      );
    }

    this.creationsByOperationId.set(operationId, { nodeId, insertedText });
  }

  prepareEdit(document: TextDocument, startOffset: number, endOffset: number, text: string): PendingSourceEdit {
    return {
      startOffset,
      endOffset,
      text,
      documentTextBefore: document.getText(),
      documentVersionBefore: document.version,
    };
  }

  /** Verify an applied undo or redo. A redo updates the text to replay next, in case the editor normalized it. */
  commitReplay(
    document: TextDocument,
    operationId: string,
    direction: ReplayDirection,
    pending: PendingSourceEdit,
  ): void {
    const creation = this.creationsByOperationId.get(operationId);
    const appliedText = getAppliedText(document, pending);
    if (!creation || appliedText === null || (direction === "redo") !== appliedText.length > 0) {
      this.creationsByOperationId.delete(operationId);
      throw new SourceEditHistoryConflict(
        "The Bicep file changed while replaying a designer action. Check the editor before continuing.",
      );
    }

    if (direction === "redo") {
      creation.insertedText = appliedText;
    }
  }
}

/** The text an applied edit put in place of its range, or null if the document changed in any other way. */
function getAppliedText(document: TextDocument, pending: PendingSourceEdit): string | null {
  const textAfter = document.getText();
  const prefix = pending.documentTextBefore.slice(0, pending.startOffset);
  const suffix = pending.documentTextBefore.slice(pending.endOffset);
  if (
    document.version === pending.documentVersionBefore ||
    textAfter.length < prefix.length + suffix.length ||
    !textAfter.startsWith(prefix) ||
    !textAfter.endsWith(suffix)
  ) {
    return null;
  }

  const appliedText = textAfter.slice(prefix.length, textAfter.length - suffix.length);
  return normalizeLineEndings(appliedText) === normalizeLineEndings(pending.text) ? appliedText : null;
}
