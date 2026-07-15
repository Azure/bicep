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

// ── Server-driven visual graph layout ──
// The webview submits the graph it currently displays; the server returns a complete patch
// delta transforming it into the latest graph. Patches are forwarded to the webview as-is, so
// they are left untyped here; the typed model lives in the visual-designer webview package.
// The method name and shapes mirror the C# `VisualGraphUpdateProtocol` on the language server.
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

// Resolves a node's source location on demand (e.g. on double-click), so the canonical graph never
// carries volatile range/filePath data. `found` is false when the node no longer exists in the live
// compilation, in which case `filePath`/`range` are null and nothing is revealed.
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

export interface GetDeploymentDataRequest {
  textDocument: TextDocumentIdentifier;
}

export interface GetDeploymentDataResponse {
  localDeployEnabled: boolean;
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

export interface LocalDeployRequest {
  textDocument: TextDocumentIdentifier;
}

export interface LocalDeploymentOperationError {
  code: string;
  message: string;
  target: string;
}

export interface LocalDeploymentOperationContent {
  resourceName: string;
  provisioningState: string;
  error?: LocalDeploymentOperationError;
}

interface LocalDeploymentContent {
  provisioningState: string;
  outputs: Record<string, unknown>;
  error?: LocalDeploymentOperationError;
}

export interface LocalDeployResponse {
  deployment: LocalDeploymentContent;
  operations: LocalDeploymentOperationContent[];
}

export const localDeployRequestType = new ProtocolRequestType<
  LocalDeployRequest,
  LocalDeployResponse,
  never,
  void,
  void
>("bicep/localDeploy");

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
  expiresOnTimestamp: string | undefined;
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

export interface BicepExternalSourceParams {
  target: string; // The full module reference to get source for
  requestedSourceFile: string | undefined; // The relative source path of the file in the module to get source for
}

export interface BicepExternalSourceResponse {
  content: string | undefined;
  error: string | undefined;
}

export const bicepExternalSourceRequestType = new ProtocolRequestType<
  BicepExternalSourceParams,
  BicepExternalSourceResponse,
  never,
  void,
  void
>("textDocument/bicepExternalSource");

export interface InsertResourceParams {
  textDocument: TextDocumentIdentifier;
  position: Position;
  resourceId: string;
}

export const insertResourceRequestType = new ProtocolNotificationType<InsertResourceParams, void>(
  "textDocument/insertResource",
);

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
  languageId: string;
}

export interface BicepDecompileForPasteCommandResult {
  decompileId: string;
  output: string;
  errorMessage?: string;
  pasteContext?: "none" | "string";
  // undefined if can't be pasted
  pasteType: undefined | "fullTemplate" | "resource" | "resourceList" | "jsonValue" | "bicepValue" | "fullParams";
  bicep?: string;
  disclaimer?: string;
}
