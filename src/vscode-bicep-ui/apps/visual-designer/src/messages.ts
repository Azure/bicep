// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// ── Notification: Extension → Webview ──
// Sent when the deployment graph changes (initial load + on every file edit)
export const DEPLOYMENT_GRAPH_NOTIFICATION = "deploymentGraph";

export interface DeploymentGraphPayload {
  documentPath: string;
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
// Sent when the webview has initialized and is ready to receive data
export const READY_NOTIFICATION = "ready";
