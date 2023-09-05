// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ErrorResponse } from "@azure/arm-resources";

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

export interface DeployResult {
  success: boolean;
  error?: ErrorResponse;
}

type DeploymentScopeBase = {
  portalUrl: string;
  tenantId: string;
};

export type DeploymentScope =
  | (DeploymentScopeBase & {
      scopeType: "resourceGroup";
      subscriptionId: string;
      resourceGroup: string;
    })
  | (DeploymentScopeBase & {
      scopeType: "subscription";
      location: string;
      subscriptionId: string;
    });

export type DeploymentScopeType = "resourceGroup" | "subscription";

export interface DeployPaneState {
  scope: DeploymentScope;
}

export type UntypedError = unknown;
