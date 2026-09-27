// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { TextDocument, WorkspaceEdit } from "vscode";

import crypto from "crypto";

export type ReplayDirection = "undo" | "redo";

/** The document no longer matches what the history expects, so replaying could change unrelated source. */
export class SourceEditHistoryConflict extends Error {
  constructor(message: string) {
    super(message);
    this.name = "SourceEditHistoryConflict";
  }
}

/** A validated insertion that is about to be applied. `recordInsertion` checks the result against it. */
export interface PendingInsertion {
  offset: number;
  text: string;
  documentTextBefore: string;
  documentVersionBefore: number;
}

/** An applied designer insertion that can be undone and redone. */
interface TrackedInsertion {
  offset: number;
  /** The text as the document actually contains it, after any line-ending normalization by the editor. */
  text: string;
  documentHashWithoutInsertion: string;
  documentHashWithInsertion: string;
  isInserted: boolean;
}

/** The single edit that undoes or redoes a tracked insertion. */
export interface ReplayPlan {
  operationId: string;
  direction: ReplayDirection;
  startOffset: number;
  endOffset: number;
  newText: string;
  expectedHashAfterReplay: string;
  documentVersionBeforeReplay: number;
}

function sha256(text: string): string {
  return crypto.createHash("sha256").update(text).digest("hex");
}

function normalizeLineEndings(text: string): string {
  return text.replace(/\r\n?|\n/g, "\n");
}

/**
 * Tracks designer-owned single-document insertions without relying on the focused VS Code editor.
 * A newer edit to this document invalidates replay rather than risking an unrelated source change.
 */
export class SourceEditHistory {
  private readonly insertionsByOperationId = new Map<string, TrackedInsertion>();
  /** The document version after the last designer edit. Any other version means someone else edited the file. */
  private lastKnownDocumentVersion: number | null = null;
  private currentEpoch = 0;

  /** Increments whenever tracked insertions are discarded, so the webview can tell its source steps are stale. */
  get epoch(): number {
    return this.currentEpoch;
  }

  hasOperation(operationId: string): boolean {
    return this.insertionsByOperationId.has(operationId);
  }

  prepareInsertion(document: TextDocument, edit: WorkspaceEdit): PendingInsertion {
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

    const textEdit = entry[1][0];

    return {
      offset: document.offsetAt(textEdit.range.start),
      text: textEdit.newText,
      documentTextBefore: document.getText(),
      documentVersionBefore: document.version,
    };
  }

  /** Verify an applied insertion and start tracking it. Returns the current epoch. */
  recordInsertion(document: TextDocument, operationId: string, pending: PendingInsertion): number {
    const documentTextAfter = document.getText();
    const textBeforeOffset = pending.documentTextBefore.slice(0, pending.offset);
    const textAfterOffset = pending.documentTextBefore.slice(pending.offset);
    const insertedText = documentTextAfter.slice(pending.offset, documentTextAfter.length - textAfterOffset.length);
    if (
      document.version === pending.documentVersionBefore ||
      !documentTextAfter.startsWith(textBeforeOffset) ||
      !documentTextAfter.endsWith(textAfterOffset) ||
      !insertedText ||
      normalizeLineEndings(insertedText) !== normalizeLineEndings(pending.text)
    ) {
      this.invalidate();
      throw new SourceEditHistoryConflict(
        "The resource was created, but the Bicep file changed unexpectedly. Use the editor's Undo command.",
      );
    }

    // Someone else edited the file since the last designer edit, so earlier insertions can no longer be replayed.
    if (this.lastKnownDocumentVersion !== null && pending.documentVersionBefore !== this.lastKnownDocumentVersion) {
      this.invalidate();
    }

    this.insertionsByOperationId.set(operationId, {
      offset: pending.offset,
      text: insertedText,
      documentHashWithoutInsertion: sha256(pending.documentTextBefore),
      documentHashWithInsertion: sha256(documentTextAfter),
      isInserted: true,
    });
    this.lastKnownDocumentVersion = document.version;
    return this.currentEpoch;
  }

  planReplay(document: TextDocument, operationId: string, direction: ReplayDirection): ReplayPlan {
    const insertion = this.insertionsByOperationId.get(operationId);
    if (!insertion || this.lastKnownDocumentVersion === null) {
      throw new SourceEditHistoryConflict("This designer source edit is no longer available to undo or redo.");
    }

    const isUndo = direction === "undo";
    const expectedHashBeforeReplay = isUndo
      ? insertion.documentHashWithInsertion
      : insertion.documentHashWithoutInsertion;
    if (document.version !== this.lastKnownDocumentVersion || sha256(document.getText()) !== expectedHashBeforeReplay) {
      this.invalidate();
      throw new SourceEditHistoryConflict(
        "The Bicep file changed outside the designer. Its undo history was cleared; use the editor's Undo command.",
      );
    }

    if (insertion.isInserted !== isUndo) {
      throw new SourceEditHistoryConflict("This designer source edit has already been undone or redone.");
    }

    const startOffset = insertion.offset;
    const endOffset = isUndo ? startOffset + insertion.text.length : startOffset;
    if (isUndo && document.getText().slice(startOffset, endOffset) !== insertion.text) {
      this.invalidate();
      throw new SourceEditHistoryConflict("The resource declaration no longer matches the original insertion.");
    }

    return {
      operationId,
      direction,
      startOffset,
      endOffset,
      newText: isUndo ? "" : insertion.text,
      expectedHashAfterReplay: isUndo ? insertion.documentHashWithoutInsertion : insertion.documentHashWithInsertion,
      documentVersionBeforeReplay: document.version,
    };
  }

  /** Verify an applied replay edit and record the new state. Returns the current epoch. */
  commitReplay(document: TextDocument, plan: ReplayPlan): number {
    if (
      document.version === plan.documentVersionBeforeReplay ||
      sha256(document.getText()) !== plan.expectedHashAfterReplay
    ) {
      this.invalidate();
      throw new SourceEditHistoryConflict(
        "The Bicep file changed while replaying a designer action. Check the editor before continuing.",
      );
    }

    const insertion = this.insertionsByOperationId.get(plan.operationId);
    if (!insertion) {
      throw new SourceEditHistoryConflict("This designer source edit is no longer available.");
    }

    insertion.isInserted = plan.direction === "redo";
    this.lastKnownDocumentVersion = document.version;
    return this.currentEpoch;
  }

  /** Discard every tracked insertion and start a new epoch. Returns the new epoch. */
  invalidate(): number {
    this.insertionsByOperationId.clear();
    this.lastKnownDocumentVersion = null;
    return ++this.currentEpoch;
  }
}
