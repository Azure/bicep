// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  ProtocolRequestType,
  Range,
  TextDocumentIdentifier,
} from "vscode-languageserver-protocol";

export interface DeploymentGraphParams {
  textDocument: TextDocumentIdentifier;
}

export interface DeploymentGraphNode {
  id: string;
  type: string;
  isCollection: boolean;
  range: Range;
  hasChildren: boolean;
  hasError: boolean;
  filePath: string | null;
}

export interface DeploymentGraphEdge {
  sourceId: string;
  targetId: string;
}

export interface DeploymentGraph {
  nodes: DeploymentGraphNode[];
  edges: DeploymentGraphEdge[];
  errorCount: number;
}

export const deploymentGraphRequestType = new ProtocolRequestType<
  DeploymentGraphParams,
  DeploymentGraph | null,
  never,
  void,
  void
>("textDocument/deploymentGraph");

export interface BicepCacheParams {
  target: string;
}

export interface BicepCacheResponse {
  content: string;
}

export const bicepCacheRequestType = new ProtocolRequestType<
  BicepCacheParams,
  BicepCacheResponse,
  never,
  void,
  void
>("textDocument/bicepCache");
