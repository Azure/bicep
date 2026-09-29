// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "./types";

import { defineNotification, defineRequest, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

// ── Document ──
// `ready` and `documentDidChange` are two halves of one exchange: the webview announces it is
// mounted, and the host answers by sending the document and re-announcing it on every edit.

/** "The webview has mounted; start sending me the document." */
export const ready = defineNotification("ready");

export interface DocumentDidChangeParams {
  documentUri: string;
}

/** "The document changed; re-fetch whatever you derive from it." */
export const documentDidChange = defineNotification<DocumentDidChangeParams>("documentDidChange");

/**
 * Sent on every edit, without waiting for `documentDidChange`, which is debounced. Anything derived from
 * the previous content, such as which designer steps can be replayed, is stale until the next graph update.
 */
export const documentDidEdit = defineNotification("documentDidEdit");

// ── Resource editing setting ──
// The wire names still say `resourceCreation` because creation was the first gated action.

export const getResourceEditingEnablement = defineRequest<void, boolean>("resourceCreation/isEnabled");

export const resourceEditingEnablementDidChange = defineNotification<boolean>("resourceCreation/enablementDidChange");

// ── Motion policy ──
// The host resolves the effective policy from the VS Code setting and the OS reduced-motion preference.

export type MotionPolicy = "system" | "reduce" | "animate";

export const getMotionPolicy = defineRequest<void, MotionPolicy>("motionPolicy/get");

export const motionPolicyDidChange = defineNotification<MotionPolicy>("motionPolicy/didChange");

// ── Source locations ──

interface Position {
  line: number;
  character: number;
}

export interface Range {
  start: Position;
  end: Position;
}

// ── Notification: Webview → Extension ──
// Sent when the user wants to reveal a node's source. The canonical graph carries no source
// locations — they shift on edits that change nothing visible, so including them would put churn in
// every diff — and the host asks the server to resolve the node id on demand instead.
export const revealNodeSource = defineNotification<RevealNodeSourceParams>("revealNodeSource");

interface RevealNodeSourceParams {
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
  symbolicName: string;
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
export const replaySourceEdit = defineRequest<ReplaySourceEditParams, ReplaySourceEditResult>(
  "undoHistory/replaySourceEdit",
);

export interface ReplaySourceEditParams extends SourceStepReference {
  version: 1;
}

export type ReplaySourceEditResult = ReplaySourceEditParams;

/**
 * Kept, like `CreateResourceErrorResult` below, to document the codes the extension can send.
 * `replayUnavailable` means the step can no longer be replayed exactly, so it should leave the history.
 */
export interface ReplaySourceEditErrorResult {
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

// ──────────────────────────────────────────────────────────────────────────
// Server-driven graph protocol
//
// The extension announces that the graph may have changed and the webview pulls the update:
//   1. Extension → Webview: DOCUMENT_DID_CHANGE notification ("the graph may have changed").
//   2. Webview → Extension: GET_GRAPH_UPDATE request carrying the graph it currently displays.
//   3. Webview → Extension: GET_GRAPH_LAYOUT request after rendered node sizes are measured.
// ──────────────────────────────────────────────────────────────────────────

// ── Request: Webview → Extension ──
// The webview submits the graph it currently displays (null on first load) and receives a
// complete patch delta transforming it into the server's latest graph.
export const getGraphUpdate = defineRequest<GetGraphUpdateParams, GetGraphUpdateResult>("getGraphUpdate");

export interface GetGraphUpdateParams {
  current: RenderedGraph | null;
  /** The source steps in the undo history, so the host can say which can still be replayed exactly. */
  sourceSteps: SourceStepReference[];
}

export interface GetGraphUpdateResult {
  patches: GraphPatch[];
  targetScope: TargetScope | null;
  /**
   * The requested source steps that can be replayed exactly in the document this update reflects. The rest can
   * no longer be. Null when the host could not tell, so none should be offered until the next update.
   */
  replayableSourceSteps: SourceStepReference[] | null;
}

export type TargetScope = "resourceGroup" | "subscription" | "managementGroup" | "tenant";

export const getGraphLayout = defineRequest<GetGraphLayoutParams, GetGraphLayoutResult>("getGraphLayout");

export interface GetGraphLayoutParams {
  current: RenderedGraph;
}

export interface GetGraphLayoutResult {
  status: "ok" | "graphChanged" | "layoutFailed";
  patches: GraphPatch[];
}

type GraphNodeKind = "resource" | "module";

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

interface RenderedGraphEdge {
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
      fetchUpdate: (current: RenderedGraph | null, sourceSteps: SourceStepReference[]) =>
        channel.request(getGraphUpdate, { current, sourceSteps }),
      fetchGraphLayout: (current: RenderedGraph) => channel.request(getGraphLayout, { current }),
      createResource: (params: CreateResourceParams) => channel.request(createResource, params),
      replaySourceEdit: (params: ReplaySourceEditParams) => channel.request(replaySourceEdit, params),
      revealNodeSource: (nodeId: string) => channel.notify(revealNodeSource, { nodeId }),
    }),
    [channel],
  );
}
