// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProtocolRequestType, TextDocumentIdentifier } from "vscode-languageserver-protocol";

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
  value?: string;
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
  Create = 1,
  None = 2,
  Overwrite = 3,
  Update = 4,
}

export enum ParameterType {
  Array = 1,
  Bool = 2,
  Int = 3,
  Object = 4,
  String = 5,
}
