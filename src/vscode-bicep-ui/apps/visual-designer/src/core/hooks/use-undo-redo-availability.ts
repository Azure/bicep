// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReplayDirection } from "../api";
import type { HistoryStep } from "../undo-history";

import { useAtomValue } from "jotai";
import {
  isGraphChangeInProgressAtom,
  isResourceEditingEnabledAtom,
  nextRedoStepAtom,
  nextUndoStepAtom,
  replayableSourceStepKeysAtom,
} from "../atoms";
import { getSourceStepKey, isSourceStep } from "../undo-history";

export interface UndoRedoAvailability {
  canUndo: boolean;
  canRedo: boolean;
}

/**
 * Whether Undo and Redo can run now. They wait while a graph change is in progress. A step that edits Bicep
 * source also needs resource editing to be enabled, and the host must have confirmed with the latest graph
 * update that it can still replay the step exactly. Layout steps are always replayable.
 */
export function useUndoRedoAvailability(): UndoRedoAvailability {
  const nextUndoStep = useAtomValue(nextUndoStepAtom);
  const nextRedoStep = useAtomValue(nextRedoStepAtom);
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const isResourceEditingEnabled = useAtomValue(isResourceEditingEnabledAtom);
  const replayableSourceStepKeys = useAtomValue(replayableSourceStepKeysAtom);

  const canReplay = (step: HistoryStep | null, direction: ReplayDirection) =>
    step !== null &&
    !isGraphChangeInProgress &&
    (!isSourceStep(step) ||
      (isResourceEditingEnabled &&
        replayableSourceStepKeys.has(getSourceStepKey({ operationId: step.operationId, direction }))));

  return { canUndo: canReplay(nextUndoStep, "undo"), canRedo: canReplay(nextRedoStep, "redo") };
}
