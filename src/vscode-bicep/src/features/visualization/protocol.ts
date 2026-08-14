// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProtocolRequestType, Range, TextDocumentIdentifier } from "vscode-languageserver-protocol";

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
