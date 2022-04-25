// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { LanguageClient } from "vscode-languageclient/node";
import {
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

export interface BicepDeployParams {
  documentPath: string;
  parameterFilePath: string;
  id: string;
  deploymentScope: string;
  location: string;
  template: string;
  token: string;
  expiresOnTimestamp: string;
  deployId: string;
}

export interface BicepCreateConfigParams {
  destinationPath: string;
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

export interface InsertResourceParams {
  textDocument: TextDocumentIdentifier;
  position: Position;
  resourceId: string;
}

export const insertResourceRequestType = new ProtocolNotificationType<
  InsertResourceParams,
  void
>("textDocument/insertResource");

export async function sendRequestCreateConfigFile(
  client: LanguageClient,
  args: { destinationPath: string }
): Promise<boolean> {
  return await client.sendRequest("workspace/executeCommand", {
    command: "bicep.createConfigFile",
    arguments: [args],
  });
}

export interface GetRecommendedConfigLocationParams {
  bicepFilePath?: string;
}

export interface GetRecommendedConfigLocationResult {
  recommendedFolder?: string;
  error?: string;
}

export const getRecommendedConfigLocationRequestType = new ProtocolRequestType<
  GetRecommendedConfigLocationParams,
  GetRecommendedConfigLocationResult,
  never,
  void,
  void
>("bicep/getRecommendedConfigLocation");
