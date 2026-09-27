// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { HistoryStep } from "../undo-history";

import { useAtomValue } from "jotai";
import {
  isGraphChangeInProgressAtom,
  isResourceEditingEnabledAtom,
  nextRedoStepAtom,
  nextUndoStepAtom,
} from "../atoms";
import { isSourceStep } from "../undo-history";

export interface UndoRedoAvailability {
  canUndo: boolean;
  canRedo: boolean;
}

/**
 * Whether Undo and Redo can run now. They wait while a graph change is in progress, and a step that edits
 * Bicep source also needs resource editing to be enabled. Layout steps are always replayable.
 */
export function useUndoRedoAvailability(): UndoRedoAvailability {
  const nextUndoStep = useAtomValue(nextUndoStepAtom);
  const nextRedoStep = useAtomValue(nextRedoStepAtom);
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const isResourceEditingEnabled = useAtomValue(isResourceEditingEnabledAtom);

  const canReplay = (step: HistoryStep | null) =>
    step !== null && !isGraphChangeInProgress && (!isSourceStep(step) || isResourceEditingEnabled);

  return { canUndo: canReplay(nextUndoStep), canRedo: canReplay(nextRedoStep) };
}
