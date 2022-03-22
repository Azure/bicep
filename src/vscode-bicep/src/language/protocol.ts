// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  DocumentUri,
  Position,
  ProtocolNotificationType,
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
  textDocument: TextDocumentIdentifier;
  target: string;
}

export interface BicepDeploymentScopeParams {
  textDocument: TextDocumentIdentifier;
}

export interface BicepDeploymentScopeResponse {
  scope: string;
  template?: string;
  errorMessage?: string;
}

export const deploymentScopeRequestType = new ProtocolRequestType<
  BicepDeploymentScopeParams,
  BicepDeploymentScopeResponse | undefined,
  never,
  void,
  void
>("bicep/getDeploymentScope");

export interface BicepDeployParams {
  textDocument: TextDocumentIdentifier;
  parameterFilePath: string;
  id: string;
  deploymentScope: string;
  location: string;
  template: string;
}

export const bicepDeployRequestType = new ProtocolRequestType<
  BicepDeployParams,
  string,
  never,
  void,
  void
>("bicep/deploy");

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

export interface InsertResourceParams {
  textDocument: TextDocumentIdentifier;
  position: Position;
  resourceId: string;
}

export const insertResourceRequestType = new ProtocolNotificationType<
  InsertResourceParams,
  void
>("textDocument/insertResource");

export interface CreateBicepConfigParams {
  destinationPath: DocumentUri;
}

export const createBicepConfigRequestType = new ProtocolRequestType<
  CreateBicepConfigParams,
  void,
  never,
  void,
  void
>("bicep/createConfigFile");

export interface GetRecommendedConfigLocationParams {
  BicepFilePath: string | undefined;
}

export const getRecommendedConfigLocationRequestType = new ProtocolRequestType<
  GetRecommendedConfigLocationParams,
  string,
  never,
  void,
  void
>("bicep/getRecommendedConfigLocation");
