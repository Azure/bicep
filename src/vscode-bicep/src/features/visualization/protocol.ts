// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  ProtocolRequestType,
  Range,
  TextDocumentIdentifier,
  VersionedTextDocumentIdentifier,
  WorkspaceEdit,
} from "vscode-languageserver-protocol";

export type VisualGraphNodeKind = "resource" | "module";

export interface VisualGraphRenderedNode {
  id: string;
  kind: VisualGraphNodeKind;
  parentId: string | null;
  type: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
  width: number;
  height: number;
}

export interface VisualGraphRenderedEdge {
  id: string;
  sourceId: string;
  targetId: string;
}

export interface VisualGraphRendered {
  nodes: VisualGraphRenderedNode[];
  edges: VisualGraphRenderedEdge[];
}

export interface VisualGraphUpdateParams {
  textDocument: TextDocumentIdentifier;
  current: VisualGraphRendered | null;
}

export interface VisualGraphUpdateResult {
  patches: unknown[];
}

export const visualGraphUpdateRequestType = new ProtocolRequestType<
  VisualGraphUpdateParams,
  VisualGraphUpdateResult,
  never,
  void,
  void
>("textDocument/visualGraphUpdate");

export interface VisualGraphLayoutParams {
  textDocument: TextDocumentIdentifier;
  current: VisualGraphRendered;
}

export interface VisualGraphLayoutResult {
  status: "ok" | "graphChanged" | "layoutFailed";
  patches: unknown[];
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
  found: boolean;
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

export interface VisualResourceTypeCatalogItem extends VisualResourceTypeReference {
  isPreview: boolean;
}

export interface VisualResourceTypesParams {
  textDocument: TextDocumentIdentifier;
  query?: string;
  includePreview: boolean;
  pageSize: number;
  continuationToken?: string;
}

export interface VisualResourceTypesResult {
  items: VisualResourceTypeCatalogItem[];
  continuationToken?: string;
}

export const visualResourceTypesRequestType = new ProtocolRequestType<
  VisualResourceTypesParams,
  VisualResourceTypesResult,
  never,
  void,
  void
>("textDocument/visualResourceTypes");

export interface PrepareVisualResourceParams {
  textDocument: VersionedTextDocumentIdentifier;
  operationId: string;
  resourceType: VisualResourceTypeReference;
}

export interface PrepareVisualResourceResult {
  operationId: string;
  expectedNodeId: string;
  symbolicName: string;
  unresolvedRequiredProperties: string[];
  edit: WorkspaceEdit;
}

export const prepareVisualResourceRequestType = new ProtocolRequestType<
  PrepareVisualResourceParams,
  PrepareVisualResourceResult,
  never,
  void,
  void
>("textDocument/prepareVisualResource");
