// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/utils/math/geometry";
import type {
  DeploymentGraph,
  GetGraphUpdateRequest,
  GetGraphUpdateResponse,
  GraphEdge,
  GraphNode,
  GraphPatch,
  RenderedGraph,
} from "./messages";

import { useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { getDefaultStore } from "jotai";
import { useCallback, useRef } from "react";
import { nodesByIdAtom } from "@/lib/graph";
import { GET_GRAPH_UPDATE_REQUEST } from "./messages";
import { useApplyDeploymentGraph } from "./use-deployment-graph";

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

function applyPatch(graph: ClientGraph, patch: GraphPatch): void {
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
          if (value !== undefined) {
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
      // Server-side layout is not produced yet (Phase 5). ELK still positions nodes on the
      // client, so layout patches are intentionally ignored here.
      return;
    case "setErrorCount":
      graph.errorCount = patch.errorCount;
      return;
  }
}

/**
 * Translate the canonical client graph into the legacy `DeploymentGraph` shape so the existing
 * position-preserving apply path (and ELK auto-layout) can render it unchanged.
 */
function toDeploymentGraph(graph: ClientGraph): DeploymentGraph {
  return {
    nodes: [...graph.nodes.values()].map((node) => ({
      id: node.id,
      type: node.type,
      isCollection: node.isCollection,
      range: node.range,
      hasChildren: node.hasChildren,
      hasError: node.hasError,
      filePath: node.filePath ?? "",
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

/**
 * Drives the notify-then-request loop for server-driven graph updates.
 *
 * Returns a function to call on each `documentDidChange` notification. It enforces the single
 * in-flight request + dirty-flag convergence pattern from the migration plan: only one request is
 * outstanding at a time, and notifications that arrive while a request is in flight collapse into a
 * single follow-up request. Because every response is a complete delta against the graph that was
 * submitted, applying only the latest response is always correct without version tokens.
 */
export function useGraphUpdate(getViewportCenter: () => Point): () => Promise<void> {
  const applyGraph = useApplyDeploymentGraph(getViewportCenter);
  const messageChannel = useWebviewMessageChannel();
  const clientGraphRef = useRef<ClientGraph>(createClientGraph());
  const inFlightRef = useRef(false);
  const dirtyRef = useRef(false);

  return useCallback(async () => {
    if (inFlightRef.current) {
      // A request is already outstanding; mark dirty so it issues one more round when it returns.
      dirtyRef.current = true;
      return;
    }

    inFlightRef.current = true;

    try {
      do {
        dirtyRef.current = false;

        const graph = clientGraphRef.current;
        const current: RenderedGraph | null = graph.nodes.size === 0 ? null : buildRenderedGraph(graph);
        const request: GetGraphUpdateRequest = { current };

        const response = await messageChannel.sendRequest<GetGraphUpdateResponse>({
          method: GET_GRAPH_UPDATE_REQUEST,
          params: request,
        });

        for (const patch of response.patches) {
          applyPatch(graph, patch);
        }

        applyGraph(toDeploymentGraph(graph));
      } while (dirtyRef.current);
    } finally {
      inFlightRef.current = false;
    }
  }, [applyGraph, messageChannel]);
}
