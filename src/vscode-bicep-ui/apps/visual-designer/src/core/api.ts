// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "./types";

import { defineNotification, defineRequest, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

// ── Document ──
// `webview/ready` and `document/didChange` are two halves of one exchange: the webview announces it
// is mounted, and the host answers with the settings and the document, and re-announces the document on
// every edit.

/** "The webview has mounted; start sending me the document." */
export const ready = defineNotification("webview/ready");

export interface DocumentDidChangeParams {
  documentUri: string;
}

/**
 * "The document may have changed; re-fetch whatever you derive from it." Sent on every change without
 * debouncing, so consumers pace their own requests.
 */
export const documentDidChange = defineNotification<DocumentDidChangeParams>("document/didChange");

// ── Settings ──
// The host sends every setting the webview depends on once it is ready, and again whenever one changes.

/** `system` follows the OS reduced-motion preference; the host resolves the VS Code setting to one of these. */
export type MotionPolicy = "system" | "reduce" | "animate";

export interface Settings {
  motionPolicy: MotionPolicy;
  /**
   * The experimental `bicep.visualizer.experimental.enableResourceEditing` setting. It gates every action
   * that edits Bicep source; the host rechecks it before applying any edit.
   */
  isResourceEditingEnabled: boolean;
  /**
   * The `bicep.visualizer.matchColorTheme` setting: color the designer with the active VS Code color
   * theme's colors instead of the curated palette for its theme kind. Appearance only.
   */
  isColorThemeMatched: boolean;
}

export const settingsDidChange = defineNotification<Settings>("settings/didChange");

// ── Source locations ──
// Sent when the user wants to reveal a node's source. The graph carries no source locations — they shift
// on edits that change nothing visible — so the host asks the server to resolve the node id on demand.
export const revealNode = defineNotification<RevealNodeParams>("document/revealNode");

interface RevealNodeParams {
  nodeId: string;
}

// ── Resource creation ──

export const createResource = defineRequest<CreateResourceParams, CreateResourceResult>("resources/create");

export interface CreateResourceParams {
  version: 1;
  operationId: string;
  resourceType: ResourceTypeReference;
}

export interface CreateResourceResult {
  version: 1;
  operationId: string;
  expectedNodeId: string;
  unresolvedRequiredProperties: string[];
  /** Set when the resource was created but the extension could not track it, so it cannot be undone. */
  historyTrackingError?: string;
}

// ── Undo history ──

export type ReplayDirection = "undo" | "redo";

/** A source step in the undo history, and which way it would be replayed next. */
export interface SourceStepReference {
  operationId: string;
  direction: ReplayDirection;
}

/** Undo or redo a source edit the designer made earlier, identified by the operation that made it. */
export const replaySourceStep = defineRequest<ReplaySourceStepParams, ReplaySourceStepResult>(
  "history/replaySourceStep",
);

export interface ReplaySourceStepParams extends SourceStepReference {
  version: 1;
}

export type ReplaySourceStepResult = ReplaySourceStepParams;

/**
 * Kept, like `CreateResourceErrorResult` below, to document the codes the extension can send.
 * `replayUnavailable` means the step can no longer be replayed exactly, so it should leave the history.
 */
export interface ReplaySourceStepErrorResult {
  code:
    | "invalidHistoryRequest"
    | "editingDisabled"
    | "replayUnavailable"
    | "documentChanged"
    | "editRejected"
    | "replayFailed";
  message: string;
  retryable: boolean;
}

/**
 * The error shape the extension host sends when resource creation fails.
 *
 * Nothing references this today: the webview shows `error.message` without distinguishing codes, and
 * the host builds these objects inline, so the interface currently enforces nothing on either side of
 * the wire. It is kept because the code enumeration is real protocol knowledge worth not losing —
 * `retryable` in particular is the signal a retry affordance would need. `export` is what keeps it
 * alive; without it `noUnusedLocals` reports it as dead.
 */
export interface CreateResourceErrorResult {
  version: 1;
  operationId?: string;
  code:
    | "unsupportedContract"
    | "invalidResourceType"
    | "duplicateOperation"
    | "editingDisabled"
    | "documentChanged"
    | "documentReadOnly"
    | "editRejected"
    | "generationFailed";
  message: string;
  retryable: boolean;
}

// ── Graph ──
// The host announces that the document may have changed and the webview pulls the graph:
//   1. Host → webview: `document/didChange`.
//   2. Webview → host: `graph/get` returns the whole graph, built from the live compilation.
//   3. Webview → host: `graph/layout` once the webview has rendered and measured the nodes.

export const getGraph = defineRequest<GetGraphParams, GetGraphResult>("graph/get");

export interface GetGraphParams {
  /** The source steps in the undo history, so the host can say which can still be replayed exactly. */
  sourceSteps: SourceStepReference[];
}

export interface GetGraphResult {
  /** Null when the host has no graph yet (the document is not compiled): keep what is shown. */
  graph: Graph | null;
  targetScope: TargetScope | null;
  /** The errors reported for the document, including those that belong to no node. */
  errorCount: number;
  /**
   * The requested source steps that can be replayed exactly in the document this graph reflects. The rest can
   * no longer be. Null when the host could not tell, so none should be offered until the next update.
   */
  replayableSourceSteps: SourceStepReference[] | null;
}

export type TargetScope = "resourceGroup" | "subscription" | "managementGroup" | "tenant";

export const layoutGraph = defineRequest<LayoutGraphParams, LayoutGraphResult>("graph/layout");

export interface LayoutGraphParams {
  graph: MeasuredGraph;
}

/**
 * `graphChanged` means the host's graph no longer matches the measured one: fetch the graph and retry.
 * `layoutFailed` means no layout was produced: keep the current positions.
 */
export interface LayoutGraphResult {
  status: "ok" | "graphChanged" | "layoutFailed";
  /** The nodes the layout engine positioned, in graph coordinates. */
  positions: NodePosition[];
  bounds: GraphBounds | null;
}

type GraphNodeKind = "resource" | "module";

/**
 * The document's graph. Nodes carry no source location: it shifts on edits that change nothing
 * visible, so the host resolves it on demand when a node is revealed.
 */
export interface Graph {
  nodes: GraphNode[];
  edges: GraphEdge[];
}

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

/** The graph as rendered, with the size measured for each node: the input to layout. */
export interface MeasuredGraph {
  nodes: MeasuredGraphNode[];
  edges: GraphEdge[];
}

export interface MeasuredGraphNode {
  id: string;
  kind: GraphNodeKind;
  parentId: string | null;
  width: number;
  height: number;
}

export interface NodePosition {
  nodeId: string;
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
/**
 * The deployment graph's operations against the extension host.
 *
 * Callers get bound methods rather than a channel and a descriptor to combine themselves, so this is
 * the only place in the feature that touches the transport, and a test can substitute the whole
 * surface by stubbing this hook.
 *
 * Only imperative calls belong here. Subscriptions stay declarative at the call site via
 * `useNotification(descriptor, handler)`, which composes better with React's lifecycle.
 */
export function useGraphApi() {
  const channel = useWebviewMessageChannel();

  return useMemo(
    () => ({
      getGraph: (sourceSteps: SourceStepReference[]) => channel.request(getGraph, { sourceSteps }),
      layoutGraph: (graph: MeasuredGraph) => channel.request(layoutGraph, { graph }),
      createResource: (params: CreateResourceParams) => channel.request(createResource, params),
      replaySourceStep: (params: ReplaySourceStepParams) => channel.request(replaySourceStep, params),
      revealNode: (nodeId: string) => channel.notify(revealNode, { nodeId }),
    }),
    [channel],
  );
}
