// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { UndoHistory } from "../undo-history";

import { atom } from "jotai";
import { EMPTY_UNDO_HISTORY } from "../undo-history";

/** The undo and redo stacks. Change them only through the pure functions in `undo-history.ts`. */
export const undoHistoryAtom = atom<UndoHistory>(EMPTY_UNDO_HISTORY);

export const nextUndoStepAtom = atom((get) => get(undoHistoryAtom).undoStack.at(-1) ?? null);

export const nextRedoStepAtom = atom((get) => get(undoHistoryAtom).redoStack.at(-1) ?? null);

/**
 * The source steps the host confirmed, with the latest graph update, it can replay exactly — keyed by
 * `getSourceStepKey`. A source step is offered only while it is in here, so a step that was just recorded or
 * replayed waits for the graph update that follows it.
 */
export const replayableSourceStepKeysAtom = atom<ReadonlySet<string>>(new Set<string>());

/** Counts edits to the Bicep file, so a graph update fetched before the latest one confirms nothing. */
export const sourceEditGenerationAtom = atom(0);

/** The Bicep file was just edited: no source step is confirmed until the graph update that follows. */
export const reportSourceEditAtom = atom(null, (_get, set) => {
  set(sourceEditGenerationAtom, (generation) => generation + 1);
  set(replayableSourceStepKeysAtom, new Set<string>());
});
