// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { GetGraphUpdateResult } from "../api";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";
import type { ResourceTypeReference } from "../types";
import type { ResourceCreationStep } from "../undo-history";

import { useStore } from "jotai";
import { useCallback } from "react";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  beginResourceCreationAtom,
  bindExpectedNodeAtom,
  cancelNodeRemovalAtom,
  discardPendingResourceAtom,
  undoHistoryAtom,
} from "../atoms";
import { discardSourceSteps, discardStaleSourceSteps, recordStep } from "../undo-history";
import { useTrackGraphChange } from "./use-track-graph-change";

/**
 * Returns a function that creates a resource where the user dropped it.
 *
 * A placeholder shows at the drop point immediately. Once the extension inserts the declaration,
 * the placeholder learns its node ID, the creation becomes an undoable history step, and the next
 * graph update replaces the placeholder with the real node at the same point.
 */
export function useResourceCreation(coordinator: GraphUpdateCoordinator<GetGraphUpdateResult>) {
  const store = useStore();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();

  return useCallback(
    (resourceType: ResourceTypeReference, origin: Point): Promise<void> => {
      const operationId = window.crypto.randomUUID();
      store.set(beginResourceCreationAtom, { operationId, resourceType, origin });

      const createResource = async () => {
        try {
          const result = await api.createResource({ version: 1, operationId, resourceType });
          const nodeId = result.expectedNodeId;

          store.set(cancelNodeRemovalAtom, nodeId);
          store.set(bindExpectedNodeAtom, { operationId, expectedNodeId: nodeId });

          if (result.historyTrackingError === undefined) {
            const step: ResourceCreationStep = {
              kind: "resourceCreation",
              operationId,
              nodeId,
              origin,
              resourceType,
              historyEpoch: result.historyEpoch,
            };
            store.set(undoHistoryAtom, (history) =>
              recordStep(discardStaleSourceSteps(history, result.historyEpoch), step),
            );
          } else {
            store.set(undoHistoryAtom, discardSourceSteps);
            console.error("Visual designer undo history tracking failed:", result.historyTrackingError);
          }
        } catch (error) {
          store.set(discardPendingResourceAtom, operationId);
          console.error(
            "Visual designer resource creation failed:",
            getErrorMessage(error, "Failed to create the resource."),
          );
        }
      };

      return trackGraphChange(() => coordinator.runMutation(createResource)).catch((error: unknown) => {
        console.error(
          "Visual designer graph refresh failed:",
          getErrorMessage(error, "The graph could not refresh after resource creation."),
        );
      });
    },
    [api, coordinator, store, trackGraphChange],
  );
}
