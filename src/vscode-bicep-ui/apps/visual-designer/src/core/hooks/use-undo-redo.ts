// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GetGraphUpdateResult, ReplayDirection } from "../api";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";
import type { NodePositions } from "../node-positions";
import type { LayoutStep, ResourceCreationStep } from "../undo-history";

import { useStore } from "jotai";
import { useCallback } from "react";
import { nodesByIdAtom } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  beginResourceCreationAtom,
  bindExpectedNodeAtom,
  cancelNodeRemovalAtom,
  discardPendingResourceAtom,
  expectNodeRemovalAtom,
  isGraphChangeInProgressAtom,
  nextRedoStepAtom,
  nextUndoStepAtom,
  replayableSourceStepKeysAtom,
  undoHistoryAtom,
} from "../atoms";
import { getAtomicNodeIds } from "../node-positions";
import { completeReplay, dropStep, forgetRemovedNodes, getSourceStepKey } from "../undo-history";
import { useTrackGraphChange } from "./use-track-graph-change";

function isReplayUnavailableError(error: unknown): boolean {
  return typeof error === "object" && error !== null && "code" in error && error.code === "replayUnavailable";
}

export interface NodeMotion {
  animateNodePositions: (positions: NodePositions) => void;
  stopNodeAnimations: () => void;
}

/**
 * Returns Undo and Redo for the undo history. Both do nothing while a graph change is in progress or
 * the history is empty, and log failures rather than throwing.
 *
 * - A layout step replays locally: nodes animate to the recorded positions.
 * - A resource creation step asks the extension to remove or reinsert the declaration, then the
 *   graph update removes or restores the node without disturbing the other nodes. It runs only if
 *   the latest graph update confirmed the extension can still replay it exactly.
 */
export function useUndoRedo(
  coordinator: GraphUpdateCoordinator<GetGraphUpdateResult>,
  { animateNodePositions, stopNodeAnimations }: NodeMotion,
) {
  const store = useStore();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();

  const replayLayoutStep = useCallback(
    (step: LayoutStep, direction: ReplayDirection) => {
      animateNodePositions(direction === "undo" ? step.before : step.after);
      store.set(undoHistoryAtom, (history) => completeReplay(history, step, direction));
    },
    [animateNodePositions, store],
  );

  /** The extension can no longer replay this step exactly, so drop it and anything that depended on it. */
  const discardSourceStep = useCallback(
    (step: ResourceCreationStep) => {
      const mountedNodeIds = getAtomicNodeIds(store);
      store.set(undoHistoryAtom, (history) => forgetRemovedNodes(dropStep(history, step), mountedNodeIds));
    },
    [store],
  );

  const replayResourceCreationStep = useCallback(
    (step: ResourceCreationStep, direction: ReplayDirection): Promise<void> => {
      const replay = async () => {
        await api.replaySourceEdit({ version: 1, operationId: step.operationId, direction });

        stopNodeAnimations();
        const isNodeMounted = store.get(nodesByIdAtom)[step.nodeId] !== undefined;
        if (direction === "undo") {
          if (isNodeMounted) {
            store.set(expectNodeRemovalAtom, step.nodeId);
          }
          // Drop the placeholder too, in case the node never arrived.
          store.set(discardPendingResourceAtom, step.operationId);
        } else {
          store.set(cancelNodeRemovalAtom, step.nodeId);
          if (!isNodeMounted) {
            // Show the placeholder at the original drop point again until the node arrives.
            store.set(beginResourceCreationAtom, {
              operationId: step.operationId,
              resourceType: step.resourceType,
              origin: step.origin,
            });
            store.set(bindExpectedNodeAtom, { operationId: step.operationId, expectedNodeId: step.nodeId });
          }
        }
        store.set(undoHistoryAtom, (history) => completeReplay(history, step, direction));
      };

      return trackGraphChange(() => coordinator.runMutation(replay)).catch((error: unknown) => {
        if (isReplayUnavailableError(error)) {
          discardSourceStep(step);
        }
        console.error(
          "Visual designer undo/redo failed:",
          getErrorMessage(error, "Failed to replay the designer source edit."),
        );
      });
    },
    [api, coordinator, discardSourceStep, stopNodeAnimations, store, trackGraphChange],
  );

  const replayNextStep = useCallback(
    async (direction: ReplayDirection): Promise<void> => {
      const step = store.get(direction === "undo" ? nextUndoStepAtom : nextRedoStepAtom);
      if (!step || store.get(isGraphChangeInProgressAtom)) {
        return;
      }

      switch (step.kind) {
        case "layout":
          replayLayoutStep(step, direction);
          return;
        case "resourceCreation":
          if (
            store.get(replayableSourceStepKeysAtom).has(getSourceStepKey({ operationId: step.operationId, direction }))
          ) {
            await replayResourceCreationStep(step, direction);
          }
          return;
      }
    },
    [replayLayoutStep, replayResourceCreationStep, store],
  );

  const undo = useCallback(() => replayNextStep("undo"), [replayNextStep]);
  const redo = useCallback(() => replayNextStep("redo"), [replayNextStep]);

  return { undo, redo };
}
