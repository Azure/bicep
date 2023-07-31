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
} from "../../../models";
import { AccessToken, TokenCredential } from "@azure/identity";
import {
  CloudError,
  Deployment,
  DeploymentOperation,
  ResourceManagementClient,
  WhatIfChange,
} from "@azure/arm-resources";

export interface UseAzureProps {
  scope?: DeploymentScope;
  templateMetadata?: TemplateMetadata;
  parametersMetadata: ParametersMetadata;
  acquireAccessToken: () => Promise<AccessToken>;
  setErrorMessage: (message?: string) => void;
}

export function useAzure(props: UseAzureProps) {
  const {
    scope,
    templateMetadata,
    parametersMetadata,
    acquireAccessToken,
    setErrorMessage,
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
      setErrorMessage(undefined);
      clearState();
      setRunning(true);

      const deployment = getDeploymentProperties(
        scope,
        templateMetadata,
        parametersMetadata.parameters,
      );
      const accessToken = await acquireAccessToken();
      const armClient = getArmClient(scope, accessToken);
      await operation(armClient, deployment);
    } catch (error) {
      setErrorMessage(`Azure operation failed: ${error}`);
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
        const result = client.deploymentOperations.listAtScope(
          getScopeId(scope),
          deploymentName,
        );
        for await (const page of result.byPage()) {
          operations.push(...page);
        }
        setOperations(operations);
      };

      let poller;
      try {
        poller = await client.deployments.beginCreateOrUpdateAtScope(
          getScopeId(scope),
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
        const response = await client.deployments.beginValidateAtScopeAndWait(
          getScopeId(scope),
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
        const response = await beginWhatIfAndWait(
          client,
          scope,
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

function parseError(error: UntypedError) {
  if (error instanceof RestError) {
    return (error.details as CloudError).error;
  }

  return {
    message: `${error}`,
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

function getScopeId(scope: DeploymentScope) {
  switch (scope.scopeType) {
    case "resourceGroup":
      return `/subscriptions/${scope.subscriptionId}/resourceGroups/${scope.resourceGroup}`;
    case "subscription":
      return `/subscriptions/${scope.subscriptionId}`;
  }
}

async function beginWhatIfAndWait(
  client: ResourceManagementClient,
  scope: DeploymentScope,
  deploymentName: string,
  deployment: Deployment,
) {
  switch (scope.scopeType) {
    case "resourceGroup":
      return await client.deployments.beginWhatIfAndWait(
        scope.resourceGroup,
        deploymentName,
        deployment,
      );
    case "subscription":
      return await client.deployments.beginWhatIfAtSubscriptionScopeAndWait(
        deploymentName,
        { ...deployment, location: scope.location },
      );
  }
}
