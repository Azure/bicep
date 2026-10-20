// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ReplayDirection, SourceStepReference } from "./api";
import type { NodePositions } from "./node-positions";
import type { ResourceTypeReference } from "./types";

/**
 * The designer's undo/redo history: one timeline of actions taken in the designer.
 *
 * Every function here is pure and returns a new history, so the history can live in a Jotai atom.
 * Edits made directly in the Bicep editor are never part of it.
 */

/** Node positions before and after a drag or Reset Layout. Replaying it never edits Bicep source. */
export interface LayoutStep {
  kind: "layout";
  before: NodePositions;
  after: NodePositions;
}

/** A resource declaration the designer inserted. The extension keeps the text needed to replay it. */
export interface ResourceCreationStep {
  kind: "resourceCreation";
  /** Identifies the insertion in the extension's source history. */
  operationId: string;
  nodeId: string;
  /** Where the user dropped the resource, so redo can place it there again. */
  origin: Point;
  resourceType: ResourceTypeReference;
}

/** A step that edits Bicep source. Replaying one requires resource editing to be enabled. */
export type SourceStep = ResourceCreationStep;

export type HistoryStep = LayoutStep | SourceStep;

export interface UndoHistory {
  /** Oldest first, so the last step is the next one to undo. */
  readonly undoStack: readonly HistoryStep[];
  /** Oldest first, so the last step is the next one to redo. */
  readonly redoStack: readonly HistoryStep[];
}

export const EMPTY_UNDO_HISTORY: UndoHistory = { undoStack: [], redoStack: [] };

export function isSourceStep(step: HistoryStep): step is SourceStep {
  return step.kind !== "layout";
}

/** Add a new step. A new action makes the redo stack unreachable, so it is cleared. */
export function recordStep(history: UndoHistory, step: HistoryStep): UndoHistory {
  return { undoStack: [...history.undoStack, step], redoStack: [] };
}

/** A new action happened that cannot itself be undone. It still makes the redo stack unreachable. */
export function recordUntrackedAction(history: UndoHistory): UndoHistory {
  return history.redoStack.length === 0 ? history : { undoStack: history.undoStack, redoStack: [] };
}

/** Record the nodes that moved between two position snapshots. Records nothing if none moved. */
export function recordLayoutChange(history: UndoHistory, before: NodePositions, after: NodePositions): UndoHistory {
  const movedBefore = new Map<string, Point>();
  const movedAfter = new Map<string, Point>();

  for (const [nodeId, from] of before) {
    const to = after.get(nodeId);
    if (to && (to.x !== from.x || to.y !== from.y)) {
      movedBefore.set(nodeId, from);
      movedAfter.set(nodeId, to);
    }
  }

  if (movedBefore.size === 0) {
    return history;
  }

  return recordStep(history, { kind: "layout", before: movedBefore, after: movedAfter });
}

/** Move `step` from the undo stack to the redo stack once it has been reverted. */
export function completeUndo(history: UndoHistory, step: HistoryStep): UndoHistory {
  if (history.undoStack.at(-1) !== step) {
    throw new Error("Undo history changed while undoing an action.");
  }

  return { undoStack: history.undoStack.slice(0, -1), redoStack: [...history.redoStack, step] };
}

/** Move `step` from the redo stack back to the undo stack once it has been reapplied. */
export function completeRedo(history: UndoHistory, step: HistoryStep): UndoHistory {
  if (history.redoStack.at(-1) !== step) {
    throw new Error("Undo history changed while redoing an action.");
  }

  return { undoStack: [...history.undoStack, step], redoStack: history.redoStack.slice(0, -1) };
}

export function completeReplay(history: UndoHistory, step: HistoryStep, direction: ReplayDirection): UndoHistory {
  return direction === "undo" ? completeUndo(history, step) : completeRedo(history, step);
}

/**
 * Keep only the parts of a layout step that refer to kept nodes. Returns an empty list if nothing is left.
 * Source steps are always kept: whether one can still be replayed is the host's call, not the graph's.
 */
function keepNodes(step: HistoryStep, isKept: (nodeId: string) => boolean): HistoryStep[] {
  if (isSourceStep(step)) {
    return [step];
  }

  const isKeptEntry = ([nodeId]: [string, Point]) => isKept(nodeId);
  const before = new Map([...step.before].filter(isKeptEntry));
  if (before.size === step.before.size) {
    return [step];
  }
  if (before.size === 0) {
    return [];
  }

  return [{ kind: "layout", before, after: new Map([...step.after].filter(isKeptEntry)) }];
}

/**
 * Drop layout history for nodes that no longer exist, so a later node that reuses an ID cannot inherit
 * stale positions. `liveNodeIds` are the nodes on the canvas plus those whose creation is pending.
 *
 * A node whose creation was undone is missing now but comes back on redo, so the redo stack keeps
 * its steps. Source steps stay even when their node is missing: an edit such as a half-typed rename
 * can remove the node for a while, and the host reports whether the step can be replayed.
 */
export function forgetRemovedNodes(history: UndoHistory, liveNodeIds: ReadonlySet<string>): UndoHistory {
  const nodeIdsRecreatedByRedo = new Set(history.redoStack.filter(isSourceStep).map((step) => step.nodeId));
  const isKeptInUndoStack = (nodeId: string) => liveNodeIds.has(nodeId);
  const isKeptInRedoStack = (nodeId: string) => liveNodeIds.has(nodeId) || nodeIdsRecreatedByRedo.has(nodeId);

  return {
    undoStack: history.undoStack.flatMap((step) => keepNodes(step, isKeptInUndoStack)),
    redoStack: history.redoStack.flatMap((step) => keepNodes(step, isKeptInRedoStack)),
  };
}

/** The source steps in the history, each with the direction it would be replayed next. */
export function getSourceStepReferences(history: UndoHistory): SourceStepReference[] {
  const references = (steps: readonly HistoryStep[], direction: ReplayDirection) =>
    steps.filter(isSourceStep).map(({ operationId }) => ({ operationId, direction }));

  return [...references(history.undoStack, "undo"), ...references(history.redoStack, "redo")];
}

/** Identifies a source step replayed in one direction, for set lookups. */
export function getSourceStepKey({ operationId, direction }: SourceStepReference): string {
  return `${direction}:${operationId}`;
}

/** Drop one step that can no longer be replayed, wherever it is. */
export function dropStep(history: UndoHistory, step: HistoryStep): UndoHistory {
  return {
    undoStack: history.undoStack.filter((candidate) => candidate !== step),
    redoStack: history.redoStack.filter((candidate) => candidate !== step),
  };
}
