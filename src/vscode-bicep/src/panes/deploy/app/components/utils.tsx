// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { DeploymentOperation } from "@azure/arm-resources";
import { ParamData, ParamDefinition, TemplateMetadata } from "./models";

export function parseTemplateJson(json: string): TemplateMetadata {
  const template = JSON.parse(json);

  const parameters: ParamDefinition[] = [];
  for (const key in template.parameters) {
    const parameter = template.parameters[key];
    parameters.push({
      name: key,
      type: parameter.type,
      defaultValue: parameter.defaultValue,
    });
  }

  return {
    template,
    parameters,
  };
}

export function parseParametersJson(json: string) {
  const data = JSON.parse(json);

  const paramValues: Record<string, ParamData> = {};
  for (const key in data.parameters) {
    paramValues[key] = {
      useDefault: false,
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

export function getPreformattedJson(input: any) {
  return (
    <pre>{JSON.stringify(input, null, 2)}</pre>
  )
}