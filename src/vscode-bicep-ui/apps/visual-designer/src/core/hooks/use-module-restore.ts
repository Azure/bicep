// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GetGraphResult } from "../api";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";

import { useStore } from "jotai";
import { useCallback } from "react";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import { beginModuleRestoreAtom, graphViewAtom, moduleRestoresAtom, settleModuleRestoreAtom } from "../atoms";

/**
 * Restore source, then request the ordinary graph refresh. This owns request progress and retry, not how the
 * arriving graph is displayed. If only the refresh failed, retry it without restoring the source again.
 */
export function useModuleRestore(coordinator: GraphUpdateCoordinator<GetGraphResult>) {
  const store = useStore();
  const api = useGraphApi();

  return useCallback(
    (nodeId: string) => {
      const previous = store.get(moduleRestoresAtom).get(nodeId);
      let sourceRestored = previous?.status === "failed" && previous.sourceRestored;
      const requestId = window.crypto.randomUUID();
      if (!store.set(beginModuleRestoreAtom, { nodeId, requestId })) {
        return;
      }

      const restore = async () => {
        let failure: string | undefined;
        try {
          if (!sourceRestored) {
            const result = await api.restoreModuleSource(nodeId);
            if (result.status === "failed") {
              failure = result.message || "The module could not be restored.";
              return;
            }
            sourceRestored = result.status === "restored";
          }

          // Restoring edits no document, so no document-change notification will request this update.
          await coordinator.requestUpdate();
          if (store.get(graphViewAtom).source.nodes.get(nodeId)?.requiresRestore) {
            failure = "The module's updated graph is not available. Try again.";
          }
        } catch (error) {
          failure = getErrorMessage(
            error,
            sourceRestored ? "Failed to load the restored module's graph." : "The module could not be restored.",
          );
          console.error("Visual designer module expansion failed:", failure);
        } finally {
          store.set(settleModuleRestoreAtom, { nodeId, requestId, failure, sourceRestored });
        }
      };

      void restore();
    },
    [api, coordinator, store],
  );
}
