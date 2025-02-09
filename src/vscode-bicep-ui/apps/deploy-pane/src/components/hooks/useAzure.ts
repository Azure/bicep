import type { CloudError, Deployment, DeploymentOperation, ErrorResponse, WhatIfChange } from "@azure/arm-resources";
import type { AccessToken, TokenCredential } from "@azure/identity";
import type {
  DeploymentScope,
  DeployState,
  ParamData,
  ParametersMetadata,
  TemplateMetadata,
  UntypedError,
} from "../../models";

import { ResourceManagementClient } from "@azure/arm-resources";
import { RestError } from "@azure/core-rest-pipeline";
import { useState } from "react";
import { getDate } from "./time";

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface UseAzureProps {
  scope?: DeploymentScope;
  templateMetadata?: TemplateMetadata;
  parametersMetadata: ParametersMetadata;
  acquireAccessToken: () => Promise<AccessToken>;
  setErrorMessage: (message?: string) => void;
}

export function useAzure(props: UseAzureProps) {
  const { scope, templateMetadata, parametersMetadata, acquireAccessToken, setErrorMessage } = props;
  const [operations, setOperations] = useState<DeploymentOperation[]>();
  const [whatIfChanges, setWhatIfChanges] = useState<WhatIfChange[]>();
  const [outputs, setOutputs] = useState<Record<string, unknown>>();
  const [deployState, setDeployState] = useState<DeployState>({});

  function getArmClient(scope: DeploymentScope, accessToken: AccessToken) {
    const tokenProvider: TokenCredential = {
      getToken: async () => accessToken,
    };

    const authenticatedSubscriptionId =
      scope.scopeType === "managementGroup" || scope.scopeType === "tenant"
        ? scope.associatedSubscriptionId
        : scope.subscriptionId;

    return new ResourceManagementClient(tokenProvider, authenticatedSubscriptionId, {
      userAgentOptions: {
        userAgentPrefix: "bicepdeploypane",
      },
      endpoint: scope.armUrl,
    });
  }

  async function doDeploymentOperation(
    scope: DeploymentScope,
    deploymentName: string | undefined,
    operation: (
      armClient: ResourceManagementClient,
      deployment: Deployment,
    ) => Promise<{ success: boolean; error?: ErrorResponse }>,
  ) {
    if (!templateMetadata) {
      return;
    }

    try {
      setErrorMessage(undefined);
      setOperations(undefined);
      setWhatIfChanges(undefined);
      setDeployState({ status: "running", name: deploymentName });

      const deployment = getDeploymentProperties(scope, templateMetadata, parametersMetadata.parameters);
      const accessToken = await acquireAccessToken();
      const armClient = getArmClient(scope, accessToken);
      const { success, error } = await operation(armClient, deployment);
      setDeployState({ status: success ? "succeeded" : "failed", name: deploymentName, error });
    } catch (error) {
      setDeployState({ status: "failed", name: deploymentName });
      setErrorMessage(`Azure operation failed: ${error}`);
    }
  }

  async function deploy() {
    if (!scope) {
      return;
    }

    const deploymentName = `bicep-deploy-${getDate()}`;
    await doDeploymentOperation(scope, deploymentName, async (client, deployment) => {
      const updateOperations = async () => {
        const operations = [];
        const result = client.deploymentOperations.listAtScope(getScopeId(scope), deploymentName);
        for await (const operation of result) {
          operations.push(operation);
        }
        setOperations(operations);
      };

      let poller;
      try {
        poller = await client.deployments.beginCreateOrUpdateAtScope(getScopeId(scope), deploymentName, deployment);

        while (!poller.isDone()) {
          await updateOperations();
          await new Promise((f) => setTimeout(f, 5000));
          await poller.poll();
        }
      } catch (e) {
        return { success: false, error: parseError(e) };
      } finally {
        if (poller) {
          // only attempt to fetch operations if the deployment ran asynchronously.
          // if it failed synchronously, then the operations API will return 404.
          await updateOperations();
        }
      }

      const finalResult = poller.getResult();
      setOutputs(finalResult?.properties?.outputs);
      return { success: true };
    });
  }

  async function validate() {
    if (!scope) {
      return;
    }

    await doDeploymentOperation(scope, undefined, async (client, deployment) => {
      try {
        const response = await client.deployments.beginValidateAtScopeAndWait(
          getScopeId(scope),
          "bicep-deploy",
          deployment,
        );

        return { success: !response.error, error: response.error };
      } catch (e) {
        return { success: false, error: parseError(e) };
      }
    });
  }

  async function whatIf() {
    if (!scope) {
      return;
    }

    await doDeploymentOperation(scope, undefined, async (client, deployment) => {
      try {
        const response = await beginWhatIfAndWait(client, scope, "bicep-deploy", deployment);

        setWhatIfChanges(response.changes);
        return { success: !response.error, error: response.error };
      } catch (e) {
        return { success: false, error: parseError(e) };
      }
    });
  }

  return {
    deployState,
    operations,
    whatIfChanges,
    outputs,
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

  const location = scope.scopeType !== "resourceGroup" ? scope.location : undefined;

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
    case "managementGroup":
      return `/providers/Microsoft.Management/managementGroups/${scope.managementGroup}`;
    case "tenant":
      return `/`;
  }
}

export function getDeploymentResourceId(scope: DeploymentScope, deploymentName: string) {
  let scopeId = getScopeId(scope);
  if (scopeId.endsWith("/")) {
    scopeId = scopeId.slice(0, -1);
  }
  return `${scopeId}/providers/Microsoft.Resources/deployments/${deploymentName}`;
}

async function beginWhatIfAndWait(
  client: ResourceManagementClient,
  scope: DeploymentScope,
  deploymentName: string,
  deployment: Deployment,
) {
  switch (scope.scopeType) {
    case "resourceGroup":
      return await client.deployments.beginWhatIfAndWait(scope.resourceGroup, deploymentName, deployment);
    case "subscription":
      return await client.deployments.beginWhatIfAtSubscriptionScopeAndWait(deploymentName, {
        ...deployment,
        location: scope.location,
      });
    case "managementGroup":
      return await client.deployments.beginWhatIfAtManagementGroupScopeAndWait(scope.managementGroup, deploymentName, {
        ...deployment,
        location: scope.location,
      });
    case "tenant":
      return await client.deployments.beginWhatIfAtTenantScopeAndWait(deploymentName, {
        ...deployment,
        location: scope.location,
      });
  }
}
