// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphUpdateResult, NodeLayout, RenderedGraph } from "../api";
import type { ClientGraph } from "../graph-model";
import type { GraphLayoutMode, GraphLayoutResult } from "../graph-update-coordinator";
import type { ResourceTypeReference } from "../types";

import { useStore } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { nodesByIdAtom } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useCanvasApi } from "../api";
import {
  beginResourceCreationAtom,
  bindExpectedNodeAtom,
  commitPendingResourcesAtom,
  failResourceCreationAtom,
  resourceNodeIsCommittingAtomFamily,
} from "../atoms";
import { centerGraphLayout, extractGraphLayout, patchMayAffectLayout } from "../graph-layout";
import { applyGraphPatch, buildRenderedGraph, createClientGraph, renderedGraphsEqual } from "../graph-model";
import { GraphUpdateCoordinator } from "../graph-update-coordinator";
import { useApplyGraph } from "./use-apply-graph";
import { useApplyGraphLayout } from "./use-apply-graph-layout";

function waitForAnimationFrame(): Promise<void> {
  return new Promise((resolve) => requestAnimationFrame(() => resolve()));
}

/** Snapshot the measured box of every mounted node, keyed by id. */
function measureNodes(store: ReturnType<typeof createStore>): Map<string, Box> {
  const renderedNodes = store.get(nodesByIdAtom);
  const boxes = new Map<string, Box>();

  for (const [nodeId, rendered] of Object.entries(renderedNodes)) {
    boxes.set(nodeId, store.get(rendered.boxAtom));
  }

  return boxes;
}

export interface CanvasController {
  requestGraphUpdate: () => Promise<void>;
  resetGraphLayout: () => Promise<void>;
  createResourceAt: (resourceType: ResourceTypeReference, origin: Point) => Promise<void>;
}

/**
 * Owns one canvas's client graph and asynchronous update lifecycle.
 *
 * Ordering lives in `GraphUpdateCoordinator`, which has no React dependency and is unit tested
 * directly — the rules there govern hazards that are impractical to force end to end.
 */
export function useCanvasController(
  getViewportCenter: () => Point,
  fitViewToBounds: (bounds: Box) => void,
): CanvasController {
  const store = useStore();
  const applyGraph = useApplyGraph(getViewportCenter);
  const applyGraphLayout = useApplyGraphLayout();
  const api = useCanvasApi();

  /**
   * The two graphs the client holds: its copy of the server's canonical graph, and the last
   * `RenderedGraph` it submitted for layout — kept so a pass can skip layout when measured sizes are
   * unchanged.
   */
  const clientGraphsRef = useRef<{ graph: ClientGraph; rendered: RenderedGraph | null }>({
    graph: createClientGraph(),
    rendered: null,
  });

  /** Expected node ids mapped to the graph position the user dropped them at. */
  const placementsRef = useRef<Map<string, Point>>(new Map());

  const fetchUpdate = useCallback(() => {
    const graph = clientGraphsRef.current.graph;
    const current: RenderedGraph | null =
      graph.nodes.size === 0 ? null : buildRenderedGraph(graph, measureNodes(store));

    return api.fetchUpdate(current);
  }, [api, store]);

  const applyUpdate = useCallback(
    async (response: GetGraphUpdateResult): Promise<{ layoutRequired: boolean }> => {
      const graph = clientGraphsRef.current.graph;
      const nodeLayouts = new Map<string, NodeLayout>();
      const newNodeOrigins = new Map<string, Point>();
      const explicitlyPlacedNodeIds = new Set(placementsRef.current.keys());
      let layoutMayBeStale = false;

      for (const patch of response.patches) {
        layoutMayBeStale ||= patchMayAffectLayout(graph, patch, explicitlyPlacedNodeIds);
        if (patch.op === "addNode") {
          const origin = placementsRef.current.get(patch.node.id);
          if (origin) {
            newNodeOrigins.set(patch.node.id, origin);
          }
        }
        applyGraphPatch(graph, nodeLayouts, patch);
      }

      const layoutRequired = layoutMayBeStale && graph.nodes.size > 0;

      if (graph.nodes.size === 0) {
        clientGraphsRef.current.rendered = null;
      }

      for (const nodeId of newNodeOrigins.keys()) {
        // Set before applyGraph mounts the node so Motion sees the compact initial state.
        store.set(resourceNodeIsCommittingAtomFamily(nodeId), true);
      }

      // Apply the new topology. Visibility is preserved for incremental edits (so nodes animate in
      // place) and gated for major changes; positions arrive with the layout.
      applyGraph(graph, newNodeOrigins);

      if (newNodeOrigins.size > 0) {
        for (const nodeId of newNodeOrigins.keys()) {
          placementsRef.current.delete(nodeId);
        }
        store.set(commitPendingResourcesAtom, new Set(newNodeOrigins.keys()));

        if (!layoutRequired) {
          // An explicitly placed node is already where the user dropped it, so no layout is owed --
          // but the graph may still be behind the visibility gate, so reveal it.
          await applyGraphLayout(new Map());
        }
      }

      return { layoutRequired };
    },
    [applyGraph, applyGraphLayout, store],
  );

  const runGraphLayout = useCallback(
    async (mode: GraphLayoutMode): Promise<GraphLayoutResult> => {
      const isReset = mode === "reset";
      const graph = clientGraphsRef.current.graph;

      if (graph.nodes.size === 0) {
        clientGraphsRef.current.rendered = null;
        return "completed";
      }

      await waitForAnimationFrame();

      const measuredGraph = buildRenderedGraph(graph, measureNodes(store));

      if (!isReset && renderedGraphsEqual(clientGraphsRef.current.rendered, measuredGraph)) {
        // Nothing was resized since the last layout, so the positions still hold. Reveal the graph in
        // case it is still behind the visibility gate. A reset skips this: the measurements are
        // unchanged when the user has only dragged nodes, which is exactly when it must still run.
        await applyGraphLayout(new Map());
        return "completed";
      }

      const layoutResponse = await api.fetchGraphLayout(measuredGraph);

      if (layoutResponse.status === "graphChanged") {
        return "graphChanged";
      }

      if (layoutResponse.status === "layoutFailed") {
        // No usable layout — reveal the graph as-is so it isn't stuck hidden.
        await applyGraphLayout(new Map());
        return "completed";
      }

      const { nodeLayouts, graphBounds } = extractGraphLayout(layoutResponse.patches);
      const { nodeLayouts: centeredNodeLayouts, bounds } = centerGraphLayout(
        nodeLayouts,
        graphBounds,
        getViewportCenter(),
      );
      clientGraphsRef.current.rendered = measuredGraph;

      // Fit the viewport to the server-computed graph bounds before the nodes settle there. A reset
      // re-arranges the same graph the user is already looking at, so it must not move their camera.
      if (bounds && !isReset) {
        fitViewToBounds(bounds);
      }

      await applyGraphLayout(centeredNodeLayouts);

      return "completed";
    },
    [api, applyGraphLayout, fitViewToBounds, getViewportCenter, store],
  );

  const [coordinator] = useState(() => new GraphUpdateCoordinator<GetGraphUpdateResult>());

  useEffect(() => {
    coordinator.setOperations({ fetchUpdate, applyUpdate, runGraphLayout });
  }, [applyUpdate, coordinator, fetchUpdate, runGraphLayout]);

  const createResourceAt = useCallback(
    (resourceType: ResourceTypeReference, origin: Point): Promise<void> => {
      const operationId = window.crypto.randomUUID();
      store.set(beginResourceCreationAtom, { operationId, resourceType, origin });

      return coordinator.runMutation(async () => {
        try {
          const { expectedNodeId } = await api.createResource({ version: 1, operationId, resourceType });

          placementsRef.current.set(expectedNodeId, origin);
          store.set(bindExpectedNodeAtom, { operationId, expectedNodeId });
        } catch (error) {
          store.set(failResourceCreationAtom, {
            operationId,
            message: getErrorMessage(error, "Failed to create the resource."),
          });
        }
      });
    },
    [api, coordinator, store],
  );

  const requestGraphUpdate = useCallback(() => coordinator.requestUpdate(), [coordinator]);
  const resetGraphLayout = useCallback(() => coordinator.requestResetGraphLayout(), [coordinator]);

  return { requestGraphUpdate, resetGraphLayout, createResourceAt };
}
