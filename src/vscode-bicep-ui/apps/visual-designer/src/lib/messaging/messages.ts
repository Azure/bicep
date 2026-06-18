// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// ── Notification: Extension → Webview ──
// Sent when the deployment graph changes (initial load + on every file edit)
export const DEPLOYMENT_GRAPH_NOTIFICATION = "deploymentGraph";

export interface DeploymentGraphPayload {
  documentPath: string;
  /** Optional basename provided by the host (for robust naming across environments). */
  documentFileName?: string;
  deploymentGraph: DeploymentGraph | null;
}

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
// Sent when the user clicks "Show errors" to open the VS Code Problems panel
export const SHOW_PROBLEMS_PANEL_NOTIFICATION = "showProblemsPanel";

// ── Notification: Webview → Extension ──
// Sent when the webview has initialized and is ready to receive data
export const READY_NOTIFICATION = "ready";

// ──────────────────────────────────────────────────────────────────────────
// Server-driven layout protocol (feature-flagged, not yet wired)
//
// Replaces the full-graph push above with a notify-then-request loop:
//   1. Extension → Webview: DOCUMENT_DID_CHANGE notification ("the graph may have changed").
//   2. Webview → Extension: GET_GRAPH_UPDATE request carrying the graph it currently displays.
//   3. Extension → Webview (response): a complete patch delta to apply.
// These contracts are defined now; the loop is wired in a later change.
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

/** Minimal node identity plus client-measured size, used as layout input. */
export interface RenderedGraphNode {
  id: string;
  kind: GraphNodeKind;
  parentId: string | null;
  width: number;
  height: number;
}

export interface RenderedGraphEdge {
  id: string;
  sourceId: string;
  targetId: string;
}

/** A node in the server's canonical graph. Sizes are measured by the webview, not sent by the server. */
export interface GraphNode {
  id: string;
  kind: GraphNodeKind;
  parentId: string | null;
  type: string;
  symbolName: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
  filePath: string | null;
  range: Range;
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

/** The mutable subset of a node that can change without altering topology (metadata-only updates). */
export interface GraphNodeChanges {
  type?: string;
  isCollection?: boolean;
  hasChildren?: boolean;
  hasError?: boolean;
  filePath?: string | null;
  range?: Range;
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
  | { op: "setErrorCount"; errorCount: number };
