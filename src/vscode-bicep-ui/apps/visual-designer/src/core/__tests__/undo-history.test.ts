// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceCreationStep, UndoHistory } from "../undo-history";

import { describe, expect, it } from "vitest";
import {
  completeRedo,
  completeUndo,
  dropStep,
  EMPTY_UNDO_HISTORY,
  forgetRemovedNodes,
  getSourceStepReferences,
  recordLayoutChange,
  recordStep,
  recordUntrackedAction,
} from "../undo-history";

function positions(entries: Record<string, [number, number]>) {
  return new Map(Object.entries(entries).map(([nodeId, [x, y]]) => [nodeId, { x, y }]));
}

function resourceCreation(nodeId: string): ResourceCreationStep {
  return {
    kind: "resourceCreation",
    operationId: `create-${nodeId}`,
    nodeId,
    origin: { x: 1, y: 2 },
    resourceType: { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2024-01-01" },
  };
}

function undoNext(history: UndoHistory): UndoHistory {
  const step = history.undoStack.at(-1);
  if (!step) {
    throw new Error("Nothing to undo.");
  }
  return completeUndo(history, step);
}

function redoNext(history: UndoHistory): UndoHistory {
  const step = history.redoStack.at(-1);
  if (!step) {
    throw new Error("Nothing to redo.");
  }
  return completeRedo(history, step);
}

describe("undo history", () => {
  it("records only the nodes that moved", () => {
    const history = recordLayoutChange(
      EMPTY_UNDO_HISTORY,
      positions({ moved: [0, 0], still: [5, 5] }),
      positions({ moved: [10, 20], still: [5, 5] }),
    );

    expect(history.undoStack).toEqual([
      { kind: "layout", before: positions({ moved: [0, 0] }), after: positions({ moved: [10, 20] }) },
    ]);
  });

  it("records nothing for a click, an unchanged move, or a node missing at the end of the gesture", () => {
    const start = positions({ a: [1, 2] });

    expect(recordLayoutChange(EMPTY_UNDO_HISTORY, start, start)).toBe(EMPTY_UNDO_HISTORY);
    expect(recordLayoutChange(EMPTY_UNDO_HISTORY, start, new Map())).toBe(EMPTY_UNDO_HISTORY);
  });

  it("moves steps between the undo and redo stacks", () => {
    const recorded = recordLayoutChange(EMPTY_UNDO_HISTORY, positions({ a: [0, 0] }), positions({ a: [1, 1] }));
    const [step] = recorded.undoStack;

    const undone = undoNext(recorded);
    expect(undone).toEqual({ undoStack: [], redoStack: [step] });

    const redone = redoNext(undone);
    expect(redone).toEqual({ undoStack: [step], redoStack: [] });
  });

  it("clears the redo stack when a new step is recorded", () => {
    const undone = undoNext(recordLayoutChange(EMPTY_UNDO_HISTORY, positions({ a: [0, 0] }), positions({ a: [1, 1] })));
    expect(undone.redoStack).toHaveLength(1);

    const afterNewMove = recordLayoutChange(undone, positions({ a: [0, 0] }), positions({ a: [5, 5] }));
    expect(afterNewMove.redoStack).toEqual([]);
  });

  it("refuses to complete a step that is not next on its stack", () => {
    const history = recordStep(recordStep(EMPTY_UNDO_HISTORY, resourceCreation("a")), resourceCreation("b"));
    const [olderStep] = history.undoStack;

    expect(() => completeUndo(history, olderStep)).toThrow(/changed while undoing/);
    expect(() => completeRedo(history, olderStep)).toThrow(/changed while redoing/);
  });

  it("forgets removed node IDs so a reused ID cannot inherit their positions", () => {
    const history = recordLayoutChange(
      EMPTY_UNDO_HISTORY,
      positions({ survivor: [1, 2], removed: [3, 4] }),
      positions({ survivor: [11, 12], removed: [13, 14] }),
    );

    const pruned = forgetRemovedNodes(history, new Set(["survivor"]));
    expect(pruned.undoStack).toEqual([
      { kind: "layout", before: positions({ survivor: [1, 2] }), after: positions({ survivor: [11, 12] }) },
    ]);

    expect(forgetRemovedNodes(pruned, new Set())).toEqual(EMPTY_UNDO_HISTORY);
  });

  it("keeps a step unchanged when all of its nodes still exist", () => {
    const history = recordLayoutChange(EMPTY_UNDO_HISTORY, positions({ a: [0, 0] }), positions({ a: [1, 1] }));

    expect(forgetRemovedNodes(history, new Set(["a"])).undoStack[0]).toBe(history.undoStack[0]);
  });

  it("keeps the creation of a node that disappeared without a designer undo, for the host to judge", () => {
    const history = recordStep(EMPTY_UNDO_HISTORY, resourceCreation("created"));

    expect(forgetRemovedNodes(history, new Set()).undoStack).toEqual([resourceCreation("created")]);
  });

  it("keeps redo steps for a node whose creation was undone, so redo can bring it back", () => {
    const created = recordStep(EMPTY_UNDO_HISTORY, resourceCreation("created"));
    const moved = recordLayoutChange(created, positions({ created: [10, 20] }), positions({ created: [60, 80] }));
    const undoneTwice = undoNext(undoNext(moved));

    // The node is gone from the canvas now that its creation is undone.
    const pruned = forgetRemovedNodes(undoneTwice, new Set());
    expect(pruned).toEqual(undoneTwice);

    // Redo the creation, then the move, in their original order.
    const creationRedone = redoNext(pruned);
    expect(creationRedone.undoStack.at(-1)?.kind).toBe("resourceCreation");
    expect(creationRedone.redoStack.at(-1)?.kind).toBe("layout");
    expect(redoNext(creationRedone).undoStack.map((step) => step.kind)).toEqual(["resourceCreation", "layout"]);
  });

  it("lists source steps with the direction each would be replayed next", () => {
    const history = undoNext(
      recordLayoutChange(
        recordStep(recordStep(EMPTY_UNDO_HISTORY, resourceCreation("a")), resourceCreation("b")),
        positions({ a: [0, 0] }),
        positions({ a: [1, 1] }),
      ),
    );

    expect(getSourceStepReferences(undoNext(history))).toEqual([
      { operationId: "create-a", direction: "undo" },
      { operationId: "create-b", direction: "redo" },
    ]);
  });

  it("drops a single step from whichever stack holds it", () => {
    const history = undoNext(recordStep(recordStep(EMPTY_UNDO_HISTORY, resourceCreation("a")), resourceCreation("b")));
    const [redoStep] = history.redoStack;

    expect(dropStep(history, redoStep)).toEqual({ undoStack: [resourceCreation("a")], redoStack: [] });
  });

  it("clears the redo stack for an action that cannot be undone", () => {
    const history = undoNext(recordStep(EMPTY_UNDO_HISTORY, resourceCreation("a")));

    expect(recordUntrackedAction(history)).toEqual(EMPTY_UNDO_HISTORY);
    expect(recordUntrackedAction(EMPTY_UNDO_HISTORY)).toBe(EMPTY_UNDO_HISTORY);
  });
});
