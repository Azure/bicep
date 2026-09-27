// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { UndoHistory } from "../undo-history";

import { atom } from "jotai";
import { EMPTY_UNDO_HISTORY } from "../undo-history";

/** The undo and redo stacks. Change them only through the pure functions in `undo-history.ts`. */
export const undoHistoryAtom = atom<UndoHistory>(EMPTY_UNDO_HISTORY);

export const nextUndoStepAtom = atom((get) => get(undoHistoryAtom).undoStack.at(-1) ?? null);

export const nextRedoStepAtom = atom((get) => get(undoHistoryAtom).redoStack.at(-1) ?? null);
