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

export interface GetDeploymentDataRequest {
  textDocument: TextDocumentIdentifier;
}

export interface GetDeploymentDataResponse {
  templateJson?: string;
  parametersJson?: string;
  errorMessage?: string;
}

export const getDeploymentDataRequestType = new ProtocolRequestType<
  GetDeploymentDataRequest,
  GetDeploymentDataResponse,
  never,
  void,
  void
>("bicep/getDeploymentData");

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
  parametersFilePath: string | undefined;
  id: string;
  deploymentScope: string;
  location: string;
  template: string;
  token: string;
  expiresOnTimestamp: string;
  deployId: string;
  deploymentName: string;
  portalUrl: string;
  parametersFileName: string | undefined;
  parametersFileUpdateOption: ParametersFileUpdateOption;
  updatedDeploymentParameters: BicepUpdatedDeploymentParameter[];
  resourceManagerEndpointUrl: string;
  audience: string;
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
  isSecure: boolean;
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
  isSecure: boolean;
  parameterType: ParameterType | undefined;
}

export enum ParametersFileUpdateOption {
  // If the user did not provide a parameters file to be used during deployment and chose to create one at the end of deployment flow
  Create = 1,
  // User select "No" to create/update parameters file at the end of deployment flow
  None = 2,
  // If the user did not provide a parameters file to be used in deployment, but chose to create one at the end of the flow and
  // file with name <bicep_file_name>.parameters.json already exists in the same folder as bicep file
  Overwrite = 3,
  // If the user provided a file to be used during deployment and chose to update it with values from current deployment at the
  // end of deployment flow
  Update = 4,
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

export interface InsertAiResourceParams {
  textDocument: TextDocumentIdentifier;
  position: Position;
  openAiEndpoint: string;
  openAiKey?: string;
  resourceType: string;
  apiVersion: string;
  scenario: string;
}

export const insertAiResourceRequestType = new ProtocolNotificationType<
  InsertAiResourceParams,
  void
>("textDocument/insertAiResource");

export interface ImportKubernetesManifestRequest {
  manifestFilePath: string;
}

export interface ImportKubernetesManifestResponse {
  bicepFilePath: string;
}

export const importKubernetesManifestRequestType = new ProtocolRequestType<
  ImportKubernetesManifestRequest,
  ImportKubernetesManifestResponse,
  never,
  void,
  void
>("bicep/importKubernetesManifest");

export interface CreateBicepConfigParams {
  destinationPath: string;
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

export interface BicepDecompileForPasteCommandParams {
  uri: string;
  bicepContent: string;
  rangeOffset: number;
  rangeLength: number;
  jsonContent: string;
  queryCanPaste: boolean;
}

export interface BicepDecompileForPasteCommandResult {
  decompileId: string;
  output: string;
  errorMessage?: string;
  pasteContext?: "none" | "string";
  // undefined if can't be pasted
  pasteType:
    | undefined
    | "fullTemplate"
    | "resource"
    | "resourceList"
    | "jsonValue"
    | "bicepValue";
  bicep?: string;
  disclaimer?: string;
}
