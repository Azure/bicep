// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ErrorResponse } from "@azure/arm-resources";

export interface TemplateMetadata {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  template: any;
  parameterDefinitions: ParamDefinition[];
}

export interface ParamDefinition {
  name: string;
  type: "string" | "int" | "bool" | "object" | "array" | object;
  allowedValues?: string[];
  defaultValue?: ParameterValue;
}

export interface ParametersMetadata {
  sourceFilePath?: string;
  parameters: Record<string, ParamData>;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type ParameterValue = any;

export interface ParamData {
  value: ParameterValue;
}

export interface DeployResult {
  success: boolean;
  error?: ErrorResponse;
}

export interface DeploymentScope {
  subscriptionId: string;
  resourceGroup: string;
}

export interface DeployPaneState {
  scope: DeploymentScope;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type UntypedError = any;

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type TelemetryProperties = Record<string, any>;
