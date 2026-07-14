// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface DeploymentGraph {
  nodes: DeploymentGraphNode[];
  edges: DeploymentGraphEdge[];
  errorCount: number;
}

export interface DeploymentGraphNode {
  id: string;
  type: string;
  isCollection: boolean;
  range: Range;
  hasChildren: boolean;
  hasError: boolean;
  filePath: string;
}

export interface DeploymentGraphEdge {
  sourceId: string;
  targetId: string;
}

export interface Range {
  start: Position;
  end: Position;
}

export interface Position {
  line: number;
  character: number;
}

// ── Notification: Webview → Extension ──
// Sent when the user wants to navigate to a source range
export const REVEAL_FILE_RANGE_NOTIFICATION = "revealFileRange";

export interface RevealFileRangePayload {
  filePath: string;
  range: Range;
}

// ── Notification: Webview → Extension ──
// Sent when the user wants to reveal a node whose source location is resolved on demand. The canonical
// graph no longer carries range/filePath, so the webview asks the host (which asks the server) to resolve
// and reveal the node by id. This keeps volatile source locations out of the per-edit graph diff.
export const REVEAL_NODE_SOURCE_NOTIFICATION = "revealNodeSource";

export interface RevealNodeSourcePayload {
  nodeId: string;
}

// ── Notification: Webview → Extension ──
// Sent when the user clicks "Show errors" to open the VS Code Problems panel
export const SHOW_PROBLEMS_PANEL_NOTIFICATION = "showProblemsPanel";

// ── Notification: Webview → Extension ──
// Sent when the webview has initialized and is ready to receive data
export const READY_NOTIFICATION = "ready";

// ──────────────────────────────────────────────────────────────────────────
// Server-driven visual graph protocol
//
// The extension announces that the graph may have changed, then the webview pulls
// topology/metadata patches and a measured layout through request/response messages:
//   1. Extension → Webview: DOCUMENT_DID_CHANGE notification ("the graph may have changed").
//   2. Webview → Extension: GET_GRAPH_UPDATE request carrying the graph it currently displays.
//   3. Webview → Extension: GET_GRAPH_LAYOUT request after rendered node sizes are measured.
// ──────────────────────────────────────────────────────────────────────────

// ── Notification: Extension → Webview ──
// "The graph may have changed; request an update when ready."
export const DOCUMENT_DID_CHANGE_NOTIFICATION = "documentDidChange";

export interface DocumentDidChangePayload {
  documentUri: string;
}

// ── Request: Webview → Extension ──
// The webview submits the graph it currently displays (null on first load) and receives a
// complete patch delta transforming it into the server's latest graph.
export const GET_GRAPH_UPDATE_REQUEST = "getGraphUpdate";

export interface GetGraphUpdateRequest {
  current: RenderedGraph | null;
}

export interface GetGraphUpdateResponse {
  patches: GraphPatch[];
}

export const GET_GRAPH_LAYOUT_REQUEST = "getGraphLayout";

export interface GetGraphLayoutRequest {
  current: RenderedGraph;
}

export interface GetGraphLayoutResponse {
  status: "ok" | "graphChanged" | "layoutFailed";
  patches: GraphPatch[];
}

export type GraphNodeKind = "resource" | "module";

/** The graph as currently rendered by the webview, sent with each update request for the server to diff against. */
export interface RenderedGraph {
  nodes: RenderedGraphNode[];
  edges: RenderedGraphEdge[];
}

/**
 * A node as currently rendered by the webview: its identity, the layout-irrelevant metadata it was rendered
 * with, and its client-measured size. The metadata travels with the request so the server can diff it
 * precisely and emit a metadata patch only when a field actually changed.
 */
export interface RenderedGraphNode {
  id: string;
  kind: GraphNodeKind;
  parentId: string | null;
  type: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
  width: number;
  height: number;
}

export interface RenderedGraphEdge {
  id: string;
  sourceId: string;
  targetId: string;
}

/**
 * A node in the server's canonical graph. Sizes are measured by the webview, not sent by the server, and
 * source locations (range/filePath) are intentionally omitted: they are resolved on demand via
 * {@link REVEAL_NODE_SOURCE_NOTIFICATION} so that whitespace-only edits never produce metadata patches.
 */
export interface GraphNode {
  id: string;
  kind: GraphNodeKind;
  parentId: string | null;
  type: string;
  symbolName: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
}

/** A directed dependency edge. Containment (parent/child) is expressed via a node's parentId, not edges. */
export interface GraphEdge {
  id: string;
  sourceId: string;
  targetId: string;
}

/** A server-computed position in graph coordinates. */
export interface NodeLayout {
  x: number;
  y: number;
}

/**
 * The size of the bounding box enclosing the whole laid-out graph. The server normalizes the graph to a
 * top-left origin, so the bounds are `{ min: (0, 0), max: (width, height) }`. The webview fits the viewport
 * to this instead of re-deriving module box extents client-side.
 */
export interface GraphBounds {
  width: number;
  height: number;
}

/** The mutable subset of a node that can change without altering topology (metadata-only updates). */
export interface GraphNodeChanges {
  type?: string | null;
  isCollection?: boolean | null;
  hasChildren?: boolean | null;
  hasError?: boolean | null;
}

/** A typed, ordered patch. A response is a complete delta as a list of these; an empty list means no change. */
export type GraphPatch =
  | { op: "clearGraph" }
  | { op: "addNode"; node: GraphNode }
  | { op: "removeNode"; nodeId: string }
  | { op: "updateNode"; nodeId: string; changes: GraphNodeChanges }
  | { op: "addEdge"; edge: GraphEdge }
  | { op: "removeEdge"; edgeId: string }
  | { op: "setNodeLayout"; nodeId: string; layout: NodeLayout }
  | { op: "setGraphBounds"; bounds: GraphBounds }
  | { op: "setErrorCount"; errorCount: number };
