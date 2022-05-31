// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

export interface BicepDeploymentStartParams {
  documentPath: string;
  parametersFilePath: string;
  id: string;
  deploymentScope: string;
  location: string;
  template: string;
  token: string;
  expiresOnTimestamp: string;
  deployId: string;
  portalUrl: string;
  parametersFileName: string;
  updateOrCreateParametersFile: ParametersFileCreateOrUpdate;
  updatedDeploymentParameters: BicepUpdatedDeploymentParameter[];
}

export interface BicepDeploymentStartResponse {
  isSuccess: boolean;
  outputMessage: string;
  viewDeploymentInPortalMessage?: string;
}

export interface BicepDeploymentWaitForCompletionParams {
  deployId: string;
  documentPath: string;
}

export interface BicepDeploymentParameter {
  name: string;
  value?: string | undefined;
  isMissingParam: boolean;
  isExpression: boolean;
  parameterType: ParameterType | undefined;
}

export interface BicepDeploymentParametersResponse {
  deploymentParameters: BicepDeploymentParameter[];
  parametersFileName: string;
  errorMessage?: string;
}

export interface BicepUpdatedDeploymentParameter {
  name: string;
  value: string;
  parameterType: ParameterType | undefined;
}

export enum ParametersFileCreateOrUpdate {
  Create = 1,
  Update = 2,
  None = 3,
}

export enum ParameterType {
  Array = 1,
  Bool = 2,
  Int = 3,
  Object = 4,
  String = 5,
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

export interface CreateBicepConfigParams {
  destinationPath: string;
}

export const createBicepConfigRequestType = new ProtocolRequestType<
  CreateBicepConfigParams,
  void,
  never,
  void,
  void
>("bicep/createConfigFile");

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
