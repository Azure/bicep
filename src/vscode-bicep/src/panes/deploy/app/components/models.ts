// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ErrorResponse } from "@azure/arm-resources";

export interface TemplateMetadata {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  template: any;
  parameterDefinitions: ParamDefinition[];
}

export interface ParamDefinition {
  type: string;
  name: string;
  defaultValue?: string;
}

export interface ParametersMetadata {
  sourceFilePath?: string;
  parameters: Record<string, ParamData>;
}

export interface ParamData {
  useDefault: boolean;
  value: string;
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
