// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export type Environment = {
  readonly suffix: string;
  readonly registryUri: string;
  readonly tenantId: string;
  readonly clientId: string;
  readonly clientSecretEnvVar: string;
  readonly templateSpecSubscriptionId: string;
  readonly resourceSuffix: string;
};

// all the live tests use the same list of environments
export const environments: Environment[] = createEnvironments();

function createEnvironments(): Environment[] {
  const environments = [
    {
      suffix: ".prod",
      registryUri: "biceptestdf.azurecr.io",
      tenantId: "72f988bf-86f1-41af-91ab-2d7cd011db47",
      clientId: "c162b385-991a-43df-b599-2d0a925e95c6",
      clientSecretEnvVar: "BICEP_SPN_PASSWORD",
      templateSpecSubscriptionId: "61e0a28a-63ed-4afc-9827-2ed09b7b30f3",
      resourceSuffix: "df",
    },
  ];

  if (isRunningInCi()) {
    environments.push({
      suffix: ".ff",
      registryUri: "biceptestff.azurecr.us",
      tenantId: "63296244-ce2c-46d8-bc36-3e558792fbee",
      clientId: "fbbb49e5-e59c-4fce-9d7b-13925a36ad7f",
      clientSecretEnvVar: "BICEP_SPN_PASSWORD_FF",
      templateSpecSubscriptionId: "e21305d9-eef2-4990-8ed2-e2748236bee3",
      resourceSuffix: "ff",
    });
  }

  return environments;
}

export function createEnvironmentOverrides(environment: Environment) {
  return {
    AZURE_TENANT_ID: environment.tenantId,
    AZURE_CLIENT_ID: environment.clientId,
    // this will be empty unless running in CI or the environment variable is not set
    AZURE_CLIENT_SECRET: process.env[environment.clientSecretEnvVar] ?? "",
  };
}

function isRunningInCi() {
  return process.env.CI === "true";
}
