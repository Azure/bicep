// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DeploymentOperation } from "@azure/arm-resources";
import type { DeploymentScopeType, ParamData, ParamDefinition, TemplateMetadata } from "../models";

function getScopeTypeFromSchema(template: Record<string, unknown>): DeploymentScopeType | undefined {
  const lookup: Record<string, DeploymentScopeType> = {
    "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#": "resourceGroup",
    "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#": "subscription",
    "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#": "managementGroup",
    "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#": "tenant",
  };

  return lookup[template["$schema"] as string];
}

export function parseTemplateJson(json: string): TemplateMetadata {
  const template = JSON.parse(json);

  const parameterDefinitions: ParamDefinition[] = [];
  for (const key in template.parameters) {
    const parameter = template.parameters[key];
    parameterDefinitions.push({
      name: key,
      type: parameter.type,
      allowedValues: parameter.allowedValues,
      defaultValue: parameter.defaultValue,
    });
  }

  const scopeType = getScopeTypeFromSchema(template);
  return {
    template,
    parameterDefinitions,
    scopeType,
  };
}

export function parseParametersJson(json: string) {
  const data = JSON.parse(json);

  const paramValues: Record<string, ParamData> = {};
  for (const key in data.parameters) {
    paramValues[key] = {
      value: data.parameters[key].value,
    };
  }

  return paramValues;
}

export function isInProgress(operation: DeploymentOperation) {
  return !isSucceeded(operation) && !isFailed(operation);
}

export function isFailed(operation: DeploymentOperation) {
  return operation.properties?.provisioningState?.toLowerCase() === "failed";
}

export function isSucceeded(operation: DeploymentOperation) {
  return operation.properties?.provisioningState?.toLowerCase() === "succeeded";
}

export function getPreformattedJson(input: unknown) {
  return <pre className="code-wrapped">{JSON.stringify(input, null, 2)}</pre>;
}
