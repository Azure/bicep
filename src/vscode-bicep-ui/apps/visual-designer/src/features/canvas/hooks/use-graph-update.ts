// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box, Point } from "@/lib/math";
import type {
  DeploymentGraph,
  GraphBounds,
  GraphEdge,
  GraphNode,
  GraphPatch,
  NodeLayout,
  Range,
  RenderedGraph,
} from "../api";
import type { ResourceTypeReference } from "../types";

import { getDefaultStore } from "jotai";
import { useCallback, useRef } from "react";
import { nodesByIdAtom } from "@/lib/graph";
import { useCanvasApi } from "../api";
import { pendingResourcesAtom, resourceCreationErrorAtom, resourceNodeIsCommittingAtomFamily } from "../atoms";
import { patchMayAffectLayout, renderedGraphsEqual } from "../utils/layout-invalidation";
import { applyGraphLayout, useApplyGraph } from "./use-apply-graph";

const store = getDefaultStore();

/**
 * The client-side mirror of the server's canonical graph. This is the graph the webview
 * "currently displays": the server diffs against it and returns a complete patch delta, so the
 * webview submits exactly this topology back on the next update request.
 */
interface ClientGraph {
  nodes: Map<string, GraphNode>;
  edges: Map<string, GraphEdge>;
  errorCount: number;
}

function createClientGraph(): ClientGraph {
  return { nodes: new Map(), edges: new Map(), errorCount: 0 };
}

function applyPatch(graph: ClientGraph, nodeLayouts: Map<string, NodeLayout>, patch: GraphPatch): void {
  switch (patch.op) {
    case "clearGraph":
      graph.nodes.clear();
      graph.edges.clear();
      graph.errorCount = 0;
      return;
    case "addNode":
      graph.nodes.set(patch.node.id, patch.node);
      return;
    case "removeNode":
      graph.nodes.delete(patch.nodeId);
      return;
    case "updateNode": {
      const node = graph.nodes.get(patch.nodeId);
      if (node) {
        // Only defined fields in `changes` override the node; the rest are left untouched.
        const next = { ...node };
        for (const [key, value] of Object.entries(patch.changes)) {
          if (value !== undefined && value !== null) {
            (next as Record<string, unknown>)[key] = value;
          }
        }
        graph.nodes.set(patch.nodeId, next);
      }
      return;
    }
    case "addEdge":
      graph.edges.set(patch.edge.id, patch.edge);
      return;
    case "removeEdge":
      graph.edges.delete(patch.edgeId);
      return;
    case "setNodeLayout":
      nodeLayouts.set(patch.nodeId, patch.layout);
      return;
    case "setGraphBounds":
      // Graph bounds drive fit-view in the layout flow, not the client graph mirror.
      return;
    case "setErrorCount":
      graph.errorCount = patch.errorCount;
      return;
  }
}

/**
 * Translate the canonical client graph into the graph shape consumed by the existing
 * position-preserving apply path.
 *
 * The canonical graph no longer carries source locations, so `range`/`filePath` are filled with empty
 * placeholders here. Reveal is driven on demand by node id (see `REVEAL_NODE_SOURCE_NOTIFICATION`),
 * which is why the empty `filePath` is intentional and not a missing value.
 */
function toDeploymentGraph(graph: ClientGraph): DeploymentGraph {
  const emptyRange: Range = { start: { line: 0, character: 0 }, end: { line: 0, character: 0 } };

  return {
    nodes: [...graph.nodes.values()].map((node) => ({
      id: node.id,
      type: node.type,
      isCollection: node.isCollection,
      range: emptyRange,
      hasChildren: node.hasChildren,
      hasError: node.hasError,
      filePath: "",
    })),
    edges: [...graph.edges.values()].map((edge) => ({
      sourceId: edge.sourceId,
      targetId: edge.targetId,
    })),
    errorCount: graph.errorCount,
  };
}

/**
 * Build the `RenderedGraph` to submit with an update request: the topology the webview holds plus
 * the size it has measured for each node (zero until the node has been laid out and measured).
 */
function buildRenderedGraph(graph: ClientGraph): RenderedGraph {
  const renderedNodes = store.get(nodesByIdAtom);

  return {
    nodes: [...graph.nodes.values()].map((node) => {
      const rendered = renderedNodes[node.id];
      const box = rendered ? store.get(rendered.boxAtom) : undefined;

      return {
        id: node.id,
        kind: node.kind,
        parentId: node.parentId,
        type: node.type,
        isCollection: node.isCollection,
        hasChildren: node.hasChildren,
        hasError: node.hasError,
        width: box ? box.max.x - box.min.x : 0,
        height: box ? box.max.y - box.min.y : 0,
      };
    }),
    edges: [...graph.edges.values()].map((edge) => ({
      id: edge.id,
      sourceId: edge.sourceId,
      targetId: edge.targetId,
    })),
  };
}

function waitForAnimationFrame(): Promise<void> {
  return new Promise((resolve) => requestAnimationFrame(() => resolve()));
}

function collectNodeLayouts(patches: GraphPatch[]): Map<string, NodeLayout> {
  const nodeLayouts = new Map<string, NodeLayout>();

  for (const patch of patches) {
    if (patch.op === "setNodeLayout") {
      nodeLayouts.set(patch.nodeId, patch.layout);
    }
  }

  return nodeLayouts;
}

/** Pick the graph bounds out of a layout response, if the server emitted them. */
function collectGraphBounds(patches: GraphPatch[]): GraphBounds | null {
  let bounds: GraphBounds | null = null;

  for (const patch of patches) {
    if (patch.op === "setGraphBounds") {
      bounds = patch.bounds;
    }
  }

  return bounds;
}

function centerGraphLayout(
  nodeLayouts: Map<string, NodeLayout>,
  graphBounds: GraphBounds | null,
  viewportCenter: Point,
): { nodeLayouts: Map<string, NodeLayout>; bounds: Box | null } {
  if (!graphBounds) {
    return { nodeLayouts, bounds: null };
  }

  const offsetX = viewportCenter.x - graphBounds.width / 2;
  const offsetY = viewportCenter.y - graphBounds.height / 2;
  const centeredLayouts = new Map<string, NodeLayout>();

  for (const [nodeId, layout] of nodeLayouts) {
    centeredLayouts.set(nodeId, { x: layout.x + offsetX, y: layout.y + offsetY });
  }

  return {
    nodeLayouts: centeredLayouts,
    bounds: {
      min: { x: offsetX, y: offsetY },
      max: { x: offsetX + graphBounds.width, y: offsetY + graphBounds.height },
    },
  };
}

/**
 * Drives the notify-then-request loop for server-driven graph updates.
 *
 * Returns a function to call on each `documentDidChange` notification. It enforces the single
 * in-flight request + dirty-flag convergence pattern from the migration plan: only one request is
 * outstanding at a time, and notifications that arrive while a request is in flight collapse into a
 * single follow-up request. Because every response is a complete delta against the graph that was
 * submitted, applying only the latest response is always correct without version tokens.
 */
export interface GraphUpdateActions {
  requestGraphUpdate: () => Promise<void>;
  createResource: (resourceType: ResourceTypeReference, origin: Point) => Promise<void>;
  /**
   * Re-run layout for the current graph and apply it, bypassing the
   * "sizes unchanged since last layout" short-circuit so it re-lays out (and
   * animates) even after the user has only dragged nodes around. Backs the Reset
   * Layout button. Shares the single in-flight slot with {@link requestGraphUpdate},
   * so it never races a concurrent document-change update.
   */
  resetLayout: () => Promise<void>;
}

export function useGraphUpdate(
  getViewportCenter: () => Point,
  fitViewToBounds: (bounds: Box) => void,
): GraphUpdateActions {
  const applyGraph = useApplyGraph(getViewportCenter);
  const api = useCanvasApi();
  const clientGraphRef = useRef<ClientGraph>(createClientGraph());
  const lastLayoutInputRef = useRef<RenderedGraph | null>(null);
  const inFlightRef = useRef(false);
  const dirtyRef = useRef(false);
  const forceLayoutRef = useRef(false);
  const mutationInFlightRef = useRef(false);
  const mutationQueueRef = useRef<Promise<void>>(Promise.resolve());
  const pendingPlacementsRef = useRef<Map<string, Point>>(new Map());

  const requestGraphLayout = useCallback(
    async (force = false) => {
      const graph = clientGraphRef.current;

      if (graph.nodes.size === 0) {
        lastLayoutInputRef.current = null;
        return;
      }

      await waitForAnimationFrame();

      const measuredGraph = buildRenderedGraph(graph);

      if (!force && renderedGraphsEqual(lastLayoutInputRef.current, measuredGraph)) {
        // Sizes are unchanged since the last layout, so positions still hold.
        // Just make sure the graph is revealed in case it was hidden.
        await applyGraphLayout(new Map());
        return;
      }

      const layoutResponse = await api.fetchLayout(measuredGraph);

      if (layoutResponse.status === "graphChanged") {
        dirtyRef.current = true;
        return;
      }

      if (layoutResponse.status === "layoutFailed") {
        // No usable layout — reveal the graph as-is so it isn't stuck hidden.
        await applyGraphLayout(new Map());
        return;
      }

      const { nodeLayouts, bounds } = centerGraphLayout(
        collectNodeLayouts(layoutResponse.patches),
        collectGraphBounds(layoutResponse.patches),
        getViewportCenter(),
      );
      lastLayoutInputRef.current = measuredGraph;

      // Fit the viewport to the server-computed graph bounds before the nodes settle there. Reset Layout
      // (force) only re-runs the layout and must not touch the user's pan/zoom, so it skips the fit.
      if (bounds && !force) {
        fitViewToBounds(bounds);
      }

      await applyGraphLayout(nodeLayouts);
    },
    [fitViewToBounds, getViewportCenter, api],
  );

  const requestGraphUpdate = useCallback(async () => {
    if (mutationInFlightRef.current) {
      dirtyRef.current = true;
      return;
    }

    if (inFlightRef.current) {
      // A request is already outstanding; mark dirty so it issues one more round when it returns.
      dirtyRef.current = true;
      return;
    }

    inFlightRef.current = true;

    try {
      do {
        // A forced layout (Reset Layout) takes priority over a normal update pass: re-run the
        // graph layout without the size-unchanged short-circuit, then fall through to drain any
        // document-change update that arrived in the meantime.
        if (forceLayoutRef.current) {
          forceLayoutRef.current = false;
          await requestGraphLayout(true);
          continue;
        }

        dirtyRef.current = false;

        const graph = clientGraphRef.current;
        const current: RenderedGraph | null = graph.nodes.size === 0 ? null : buildRenderedGraph(graph);
        const response = await api.fetchUpdate(current);

        if (mutationInFlightRef.current) {
          // The mutation response carries the expected node ID needed to correlate placement. A graph response
          // that completes first may already contain that node, so discard it and let the mutation's finally
          // block request a fresh update after recording the placement.
          dirtyRef.current = true;
          return;
        }

        const nodeLayouts = new Map<string, NodeLayout>();
        const newNodeOrigins = new Map<string, Point>();
        const explicitlyPlacedNodeIds = new Set(pendingPlacementsRef.current.keys());
        let layoutMayBeStale = false;

        for (const patch of response.patches) {
          layoutMayBeStale ||= patchMayAffectLayout(graph, patch, explicitlyPlacedNodeIds);
          if (patch.op === "addNode") {
            const origin = pendingPlacementsRef.current.get(patch.node.id);
            if (origin) {
              newNodeOrigins.set(patch.node.id, origin);
            }
          }
          applyPatch(graph, nodeLayouts, patch);
        }

        const shouldMeasureLayout = layoutMayBeStale && graph.nodes.size > 0;

        if (graph.nodes.size === 0) {
          lastLayoutInputRef.current = null;
        }

        // Apply the new topology. Visibility is preserved for incremental
        // edits (so nodes animate in place) and gated for major changes;
        // positions arrive in the layout phase below.
        if (newNodeOrigins.size > 0) {
          for (const nodeId of newNodeOrigins.keys()) {
            // Set before applyGraph mounts the node so Motion sees the compact initial state.
            store.set(resourceNodeIsCommittingAtomFamily(nodeId), true);
          }
        }
        applyGraph(toDeploymentGraph(graph), newNodeOrigins);

        if (newNodeOrigins.size > 0) {
          for (const nodeId of newNodeOrigins.keys()) {
            pendingPlacementsRef.current.delete(nodeId);
          }
          store.set(pendingResourcesAtom, (pending) =>
            pending.filter((resource) => !resource.expectedNodeId || !newNodeOrigins.has(resource.expectedNodeId)),
          );
        }

        if (shouldMeasureLayout) {
          await requestGraphLayout();
        } else if (newNodeOrigins.size > 0) {
          await applyGraphLayout(new Map());
        }
      } while (dirtyRef.current || forceLayoutRef.current);
    } finally {
      inFlightRef.current = false;
    }
  }, [applyGraph, requestGraphLayout, api]);

  const resetLayout = useCallback(async () => {
    forceLayoutRef.current = true;

    if (inFlightRef.current) {
      // A request is already outstanding; the in-flight loop drains forceLayoutRef before it
      // releases the lock, so the reset runs there instead of racing it.
      return;
    }

    await requestGraphUpdate();
  }, [requestGraphUpdate]);

  const createResource = useCallback(
    (resourceType: ResourceTypeReference, origin: Point): Promise<void> => {
      const operationId = window.crypto.randomUUID();
      store.set(pendingResourcesAtom, (pending) => [...pending, { operationId, resourceType, origin }]);
      store.set(resourceCreationErrorAtom, null);

      const execute = async () => {
        mutationInFlightRef.current = true;

        try {
          const response = await api.createResource({
            version: 1,
            operationId,
            resourceType,
          });

          pendingPlacementsRef.current.set(response.expectedNodeId, origin);
          store.set(pendingResourcesAtom, (pending) =>
            pending.map((resource) =>
              resource.operationId === operationId
                ? { ...resource, expectedNodeId: response.expectedNodeId }
                : resource,
            ),
          );
        } catch (error) {
          store.set(pendingResourcesAtom, (pending) =>
            pending.filter((resource) => resource.operationId !== operationId),
          );
          store.set(
            resourceCreationErrorAtom,
            typeof error === "object" && error !== null && "message" in error
              ? String(error.message)
              : "Failed to create the resource.",
          );
        } finally {
          mutationInFlightRef.current = false;
          await requestGraphUpdate();
        }
      };

      const queued = mutationQueueRef.current.then(execute, execute);
      mutationQueueRef.current = queued;
      return queued;
    },
    [api, requestGraphUpdate],
  );

  return { requestGraphUpdate, createResource, resetLayout };
}
