// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ErrorResponse } from "@azure/arm-resources";
import { DeploymentScope, DeploymentScopeType } from "../../azure/types";

export interface TemplateMetadata {
  scopeType?: DeploymentScopeType;
  template: Record<string, unknown>;
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

export type ParameterValue = unknown;

export interface ParamData {
  value: ParameterValue;
}

export interface DeployState {
  name?: string;
  status?: "running" | "succeeded" | "failed";
  error?: ErrorResponse;
}

export interface DeployPaneState {
  scope: DeploymentScope;
}

export type UntypedError = unknown;
export { DeploymentScope, DeploymentScopeType };
