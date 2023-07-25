// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useState } from "react";
import { RestError } from "@azure/core-rest-pipeline";
import {
  DeployResult,
  DeploymentScope,
  ParamData,
  ParametersMetadata,
  TemplateMetadata,
  UntypedError,
} from "../models";
import { AccessToken, TokenCredential } from "@azure/identity";
import {
  Deployment,
  DeploymentOperation,
  ErrorResponse,
  ResourceManagementClient,
  WhatIfChange,
} from "@azure/arm-resources";

export interface UseAzureProps {
  scope?: DeploymentScope;
  templateMetadata?: TemplateMetadata;
  parametersMetadata: ParametersMetadata;
  acquireAccessToken: () => Promise<AccessToken>;
  showErrorDialog: (callbackId: string, error: UntypedError) => void;
}

export function useAzure(props: UseAzureProps) {
  const {
    scope,
    templateMetadata,
    parametersMetadata,
    acquireAccessToken,
    showErrorDialog,
  } = props;
  const deploymentName = "bicep-deploy";
  const [operations, setOperations] = useState<DeploymentOperation[]>();
  const [whatIfChanges, setWhatIfChanges] = useState<WhatIfChange[]>();
  const [outputs, setOutputs] = useState<Record<string, unknown>>();
  const [result, setResult] = useState<DeployResult>();
  const [running, setRunning] = useState(false);

  function getArmClient(scope: DeploymentScope, accessToken: AccessToken) {
    const tokenProvider: TokenCredential = {
      getToken: async () => accessToken,
    };

    return new ResourceManagementClient(tokenProvider, scope.subscriptionId, {
      userAgentOptions: {
        userAgentPrefix: "bicepdeploypane",
      },
    });
  }

  async function doDeploymentOperation(
    scope: DeploymentScope,
    operation: (
      armClient: ResourceManagementClient,
      deployment: Deployment,
    ) => Promise<void>,
  ) {
    if (!templateMetadata) {
      return;
    }

    try {
      clearState();
      setRunning(true);

      const deployment = getDeploymentProperties(
        templateMetadata,
        parametersMetadata.parameters,
      );
      const accessToken = await acquireAccessToken();
      const armClient = getArmClient(scope, accessToken);
      await operation(armClient, deployment);
    } catch (e) {
      showErrorDialog("doDeploymentOperation", e);
    } finally {
      setRunning(false);
    }
  }

  async function deploy() {
    if (!scope) {
      return;
    }

    await doDeploymentOperation(scope, async (client, deployment) => {
      const updateOperations = async () => {
        const operations = [];
        const result = client.deploymentOperations.list(
          scope.resourceGroup,
          deploymentName,
        );
        for await (const page of result.byPage()) {
          operations.push(...page);
        }
        setOperations(operations);
      };

      let poller;
      try {
        poller = await client.deployments.beginCreateOrUpdate(
          scope.resourceGroup,
          deploymentName,
          deployment,
        );

        while (!poller.isDone()) {
          await updateOperations();
          await new Promise((f) => setTimeout(f, 5000));
          await poller.poll();
        }
      } catch (e) {
        setResult({
          success: false,
          error: parseError(e),
        });
        return;
      } finally {
        await updateOperations();
      }

      const finalResult = poller.getResult();
      setOutputs(finalResult?.properties?.outputs);
      setResult({
        success: true,
      });
    });
  }

  async function validate() {
    if (!scope) {
      return;
    }

    await doDeploymentOperation(scope, async (client, deployment) => {
      try {
        const response = await client.deployments.beginValidateAndWait(
          scope.resourceGroup,
          deploymentName,
          deployment,
        );

        setResult({
          success: !response.error,
          error: response.error,
        });
      } catch (e) {
        setResult({
          success: false,
          error: parseError(e),
        });
      }
    });
  }

  async function whatIf() {
    if (!scope) {
      return;
    }

    await doDeploymentOperation(scope, async (client, deployment) => {
      try {
        const response = await client.deployments.beginWhatIfAndWait(
          scope.resourceGroup,
          deploymentName,
          deployment,
        );

        setResult({
          success: !response.error,
          error: response.error,
        });
        setWhatIfChanges(response.changes);
      } catch (e) {
        setResult({
          success: false,
          error: parseError(e),
        });
      }
    });
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

function parseError(error: UntypedError): ErrorResponse {
  if (error instanceof RestError) {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return (error.details as any).error as ErrorResponse;
  }

  return {
    message: `${error}`,
  };
}

function getDeploymentProperties(
  metadata: TemplateMetadata,
  paramValues: Record<string, ParamData>,
): Deployment {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const parameters: Record<string, any> = {};
  for (const [key, { value }] of Object.entries(paramValues)) {
    parameters[key] = {
      value,
    };
  }

  return {
    properties: {
      mode: "Incremental",
      parameters,
      template: metadata.template,
    },
  };
}
