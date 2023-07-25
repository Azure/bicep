// Copyright (c) Microsoft Corporation.

import { ErrorResponse } from "@azure/arm-resources";

// Licensed under the MIT License.
export interface TemplateMetadata {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  template: any;
  parameters: ParamDefinition[];
}

export interface ParamDefinition {
  type: string;
  name: string;
  defaultValue?: string;
}

export interface ParamData {
  useDefault: boolean;
  value: string;
}

export type ParamsData = Record<string, ParamData>;

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