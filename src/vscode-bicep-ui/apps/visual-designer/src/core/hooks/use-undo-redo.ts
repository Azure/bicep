// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GetGraphResult, ReplayDirection } from "../api";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";
import type { LayoutStep, ResourceCreationStep } from "../undo-history";

import { useStore } from "jotai";
import { useCallback } from "react";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  beginResourceCreationAtom,
  bindExpectedNodeAtom,
  cancelNodeRemovalAtom,
  canRedoAtom,
  canUndoAtom,
  collapsedModuleIdsAtom,
  discardPendingResourceAtom,
  expectNodeRemovalAtom,
  nextRedoStepAtom,
  nextUndoStepAtom,
  pendingPlacementsAtom,
  sourceGraphAtom,
  undoHistoryAtom,
} from "../atoms";
import { getNodePositions, moveNodes, setNodePositions } from "../node-positions";
import { completeReplay, dropStep, forgetRemovedNodes } from "../undo-history";
import { useTrackGraphChange } from "./use-track-graph-change";

function isReplayUnavailableError(error: unknown): boolean {
  return typeof error === "object" && error !== null && "code" in error && error.code === "replayUnavailable";
}

/**
 * Returns Undo and Redo for the undo history. Each does nothing unless `canUndoAtom` or `canRedoAtom`
 * allows it, and logs failures rather than throwing.
 *
 * - A layout step replays locally: nodes animate to the recorded positions, including nodes hidden in a
 *   collapsed module, which are there when it opens.
 * - A resource creation step asks the extension to remove or reinsert the declaration, then the
 *   graph update removes or restores the node without disturbing the other nodes.
 */
export function useUndoRedo(coordinator: GraphUpdateCoordinator<GetGraphResult>) {
  const store = useStore();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();

  const replayLayoutStep = useCallback(
    (step: LayoutStep, direction: ReplayDirection) => {
      const positions = moveNodes(
        getNodePositions(store),
        direction === "undo" ? step.before : step.after,
        store.get(sourceGraphAtom),
        store.get(collapsedModuleIdsAtom),
      );
      setNodePositions(store, positions, "animate");
      store.set(undoHistoryAtom, (history) => completeReplay(history, step, direction));
    },
    [store],
  );

  /** The extension can no longer replay this step exactly, so drop it and anything that depended on it. */
  const discardSourceStep = useCallback(
    (step: ResourceCreationStep) => {
      const liveNodeIds = new Set([...getNodePositions(store).keys(), ...store.get(pendingPlacementsAtom).keys()]);
      store.set(undoHistoryAtom, (history) => forgetRemovedNodes(dropStep(history, step), liveNodeIds));
    },
    [store],
  );

  const replayResourceCreationStep = useCallback(
    (step: ResourceCreationStep, direction: ReplayDirection): Promise<void> => {
      const replay = async () => {
        await api.replaySourceStep({ version: 1, operationId: step.operationId, direction });

        const isNodeInGraph = store.get(sourceGraphAtom).nodes.has(step.nodeId);
        if (direction === "undo") {
          if (isNodeInGraph) {
            store.set(expectNodeRemovalAtom, step.nodeId);
          }
          // Drop the placeholder too, in case the node never arrived.
          store.set(discardPendingResourceAtom, step.operationId);
        } else {
          store.set(cancelNodeRemovalAtom, step.nodeId);
          if (!isNodeInGraph) {
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
    [api, coordinator, discardSourceStep, store, trackGraphChange],
  );

  const replayNextStep = useCallback(
    async (direction: ReplayDirection): Promise<void> => {
      const step = store.get(direction === "undo" ? nextUndoStepAtom : nextRedoStepAtom);
      if (!step || !store.get(direction === "undo" ? canUndoAtom : canRedoAtom)) {
        return;
      }

      switch (step.kind) {
        case "layout":
          replayLayoutStep(step, direction);
          return;
        case "resourceCreation":
          await replayResourceCreationStep(step, direction);
          return;
      }
    },
    [replayLayoutStep, replayResourceCreationStep, store],
  );

  const undo = useCallback(() => replayNextStep("undo"), [replayNextStep]);
  const redo = useCallback(() => replayNextStep("redo"), [replayNextStep]);

  return { undo, redo };
}
