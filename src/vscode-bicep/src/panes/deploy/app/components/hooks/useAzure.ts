// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useState } from "react";
import {
  DeployResult,
  DeploymentScope,
  ParamData,
  ParametersMetadata,
  TemplateMetadata,
} from "../../../models";
import { AccessToken } from "@azure/identity";
import {
  Deployment,
  DeploymentExtended,
  DeploymentOperation,
  WhatIfChange,
} from "@azure/arm-resources";

export interface UseAzureProps {
  scope?: DeploymentScope;
  templateMetadata?: TemplateMetadata;
  parametersMetadata: ParametersMetadata;
  acquireAccessToken: () => Promise<AccessToken>;
  setErrorMessage: (message?: string) => void;
  startDeployment: (deployment: Deployment, scope: DeploymentScope) => Promise<DeploymentExtended>;
}

export function useAzure(props: UseAzureProps) {
  const {
    scope,
    templateMetadata,
    parametersMetadata,
    setErrorMessage,
    startDeployment,
  } = props;
  const [operations, setOperations] = useState<DeploymentOperation[]>();
  const [whatIfChanges, setWhatIfChanges] = useState<WhatIfChange[]>();
  const [outputs, setOutputs] = useState<Record<string, unknown>>();
  const [result, setResult] = useState<DeployResult>();
  const [running, setRunning] = useState(false);

  async function deploy() {
    if (!scope) {
      return;
    }

    if (!templateMetadata) {
      return;
    }

    setErrorMessage(undefined);
    clearState();
    setRunning(true);

    const deployment = getDeploymentProperties(
      scope,
      templateMetadata,
      parametersMetadata.parameters,
    );

    try {
      const result = await startDeployment(deployment, scope);
      setResult({
        success: result.properties?.provisioningState === 'Succeeded',
        error: result.properties?.error,
      });
      setOutputs(result.properties?.outputs);
    } catch (e) {
      setResult({
        success: false,
        error: { message: `${e}` },
      });
    } finally {
      setRunning(false);
    }
  }

  async function validate() {
    throw `Validate is not supported`;
  }

  async function whatIf() {
    throw `What-If is not supported`;
  }

  function clearState() {
    setOutputs(undefined);
    setResult(undefined);
    setOperations(undefined);
    setWhatIfChanges(undefined);
  }

  return {
    running,
    operations,
    whatIfChanges,
    outputs,
    result,
    deploy,
    validate,
    whatIf,
  };
}

function getDeploymentProperties(
  scope: DeploymentScope,
  metadata: TemplateMetadata,
  paramValues: Record<string, ParamData>,
): Deployment {
  const parameters: Record<string, unknown> = {};
  for (const { name } of metadata.parameterDefinitions) {
    if (paramValues[name]) {
      const value = paramValues[name].value;
      parameters[name] = { value };
    }
  }

  const location =
    scope.scopeType !== "resourceGroup" ? scope.location : undefined;

  return {
    location,
    properties: {
      mode: "Incremental",
      parameters,
      template: metadata.template,
    },
  };
}