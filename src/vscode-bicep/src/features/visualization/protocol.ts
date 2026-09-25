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
  targetScope: "resourceGroup" | "subscription" | "managementGroup" | "tenant" | null;
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

export interface VisualResourceTypeNamespace {
  name: string;
  resourceTypeCount: number;
}

export interface VisualResourceTypeNamespacesParams {
  textDocument: TextDocumentIdentifier;
}

export interface VisualResourceTypeNamespacesResult {
  catalogId: string;
  namespaces: VisualResourceTypeNamespace[];
}

export const visualResourceTypeNamespacesRequestType = new ProtocolRequestType<
  VisualResourceTypeNamespacesParams,
  VisualResourceTypeNamespacesResult,
  never,
  void,
  void
>("textDocument/visualResourceTypeNamespaces");

export interface VisualResourceTypesParams {
  textDocument: TextDocumentIdentifier;
  providerNamespace?: string;
  query?: string;
  pageSize: number;
  continuationToken?: string;
}

export interface VisualResourceTypesResult {
  catalogId: string;
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

export interface CreateResourceDeclarationInsertionParams {
  textDocument: VersionedTextDocumentIdentifier;
  operationId: string;
  resourceType: VisualResourceTypeReference;
}

export interface ResourceDeclarationInsertion {
  operationId: string;
  expectedNodeId: string;
  symbolicName: string;
  unresolvedRequiredProperties: string[];
  edit: WorkspaceEdit;
}

export const createResourceDeclarationInsertionRequestType = new ProtocolRequestType<
  CreateResourceDeclarationInsertionParams,
  ResourceDeclarationInsertion,
  never,
  void,
  void
>("textDocument/prepareVisualResource");
