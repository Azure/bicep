// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ErrorResponse } from "@azure/arm-resources";

type DeploymentScopeBase<T> = {
  armUrl: string;
  portalUrl: string;
  tenantId: string;
} & T;

export type DeploymentScope = DeploymentScopeBase<
  | {
      scopeType: "resourceGroup";
      subscriptionId: string;
      resourceGroup: string;
    }
  | {
      scopeType: "subscription";
      location: string;
      subscriptionId: string;
    }
  | {
      scopeType: "managementGroup";
      associatedSubscriptionId: string;
      location: string;
      managementGroup: string;
    }
  | {
      scopeType: "tenant";
      associatedSubscriptionId: string;
      location: string;
    }
>;

export type DeploymentScopeType = "resourceGroup" | "subscription" | "managementGroup" | "tenant";

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
