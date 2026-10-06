// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { VisualizerMotionPolicy } from "./motion-policy";

import {
  ProtocolRequestType,
  Range,
  TextDocumentIdentifier,
  VersionedTextDocumentIdentifier,
  WorkspaceEdit,
} from "vscode-languageserver-protocol";

/** The settings the webview depends on, sent with `settings/didChange`. */
export interface VisualizerSettings {
  motionPolicy: VisualizerMotionPolicy;
  isResourceEditingEnabled: boolean;
}

// The extension forwards graphs between the webview and the language server without reading them.

export interface VisualGraphParams {
  textDocument: TextDocumentIdentifier;
}

export interface VisualGraphResult {
  /** Null when the document has not been compiled yet: the webview keeps what it shows. */
  graph: unknown;
  targetScope: "resourceGroup" | "subscription" | "managementGroup" | "tenant" | null;
  /** The errors reported for the document, including those that belong to no node. */
  errorCount: number;
}

export const visualGraphRequestType = new ProtocolRequestType<VisualGraphParams, VisualGraphResult, never, void, void>(
  "textDocument/visualGraph",
);

export interface VisualGraphLayoutParams {
  textDocument: TextDocumentIdentifier;
  /** The graph the webview rendered, with the size it measured for each node. */
  graph: unknown;
}

export interface VisualGraphLayoutResult {
  status: "ok" | "graphChanged" | "layoutFailed";
  positions: unknown[];
  bounds: unknown;
}

export const visualGraphLayoutRequestType = new ProtocolRequestType<
  VisualGraphLayoutParams,
  VisualGraphLayoutResult,
  never,
  void,
  void
>("textDocument/visualGraphLayout");
export interface VisualGraphNodeSourceParams {
  textDocument: TextDocumentIdentifier;
  nodeId: string;
}

export interface VisualGraphNodeSourceResult {
  filePath: string | null;
  range: Range | null;
}

export const visualGraphNodeSourceRequestType = new ProtocolRequestType<
  VisualGraphNodeSourceParams,
  VisualGraphNodeSourceResult,
  never,
  void,
  void
>("textDocument/visualGraphNodeSource");

export interface VisualResourceTypeReference {
  fullyQualifiedType: string;
  apiVersion: string;
}

export interface VisualResourceTypesParams {
  textDocument: TextDocumentIdentifier;
  /** The catalog the webview already holds, so the language server can skip sending it again. */
  knownCatalogId?: string;
}

export interface VisualResourceTypesResult {
  catalogId: string;
  /** Every type deployable at the document's target scope, or null when the catalog is unchanged. */
  resourceTypes: VisualResourceTypeReference[] | null;
}

export const visualResourceTypesRequestType = new ProtocolRequestType<
  VisualResourceTypesParams,
  VisualResourceTypesResult,
  never,
  void,
  void
>("textDocument/visualResourceTypes");
export interface VisualResourceTypeVersionsParams {
  textDocument: TextDocumentIdentifier;
  fullyQualifiedType: string;
}

export interface VisualResourceTypeVersionsResult {
  catalogId: string;
  apiVersions: string[];
}

export const visualResourceTypeVersionsRequestType = new ProtocolRequestType<
  VisualResourceTypeVersionsParams,
  VisualResourceTypeVersionsResult,
  never,
  void,
  void
>("textDocument/visualResourceTypeVersions");

export interface PrepareVisualResourceCreationParams {
  textDocument: VersionedTextDocumentIdentifier;
  operationId: string;
  resourceType: VisualResourceTypeReference;
}

export interface PrepareVisualResourceCreationResult {
  operationId: string;
  expectedNodeId: string;
  unresolvedRequiredProperties: string[];
  edit: WorkspaceEdit;
}

export const prepareVisualResourceCreationRequestType = new ProtocolRequestType<
  PrepareVisualResourceCreationParams,
  PrepareVisualResourceCreationResult,
  never,
  void,
  void
>("textDocument/prepareVisualResourceCreation");

export type VisualResourceReplayDirection = "undo" | "redo";

export interface VisualResourceReplayQuery {
  operationId: string;
  /** The graph node the creation produced, which is the resource's symbolic name. */
  nodeId: string;
  direction: VisualResourceReplayDirection;
  /** The text the creation inserted, including its surrounding newlines, as the document contains it. */
  insertedText: string;
}

export interface PrepareVisualResourceReplayParams {
  textDocument: TextDocumentIdentifier;
  replays: VisualResourceReplayQuery[];
}

export interface VisualResourceReplayEdit {
  operationId: string;
  /** The edit that replays the creation exactly against the current document, or null if that is not possible. */
  edit: { range: Range; newText: string } | null;
}

export interface PrepareVisualResourceReplayResult {
  replays: VisualResourceReplayEdit[];
}

export const prepareVisualResourceReplayRequestType = new ProtocolRequestType<
  PrepareVisualResourceReplayParams,
  PrepareVisualResourceReplayResult,
  never,
  void,
  void
>("textDocument/prepareVisualResourceReplay");
