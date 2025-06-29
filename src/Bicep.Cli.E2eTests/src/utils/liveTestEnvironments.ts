// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export type EnvironmentOverrides = Record<string, string>;

interface LiveTestEnvironment {
  suffix: string;
  registryUri: string;
  templateSpecSubscriptionId: string;
  resourceSuffix: string;
  environmentOverrides: EnvironmentOverrides;
}

export const prod: LiveTestEnvironment = {
  suffix: ".prod",
  registryUri: "biceptestdf.azurecr.io",
  templateSpecSubscriptionId: "61e0a28a-63ed-4afc-9827-2ed09b7b30f3",
  resourceSuffix: "df",
  environmentOverrides: {},
};

export const fairfax: LiveTestEnvironment = {
  suffix: ".ff",
  registryUri: "biceptestff2.azurecr.us",
  templateSpecSubscriptionId: "d1d65353-7d87-447b-8daa-89e868034b2a",
  resourceSuffix: "ff",
  environmentOverrides: {},
};

export function getEnvironment(): LiveTestEnvironment {
  const environmentName = process.env.TEST_ENVIRONMENT;

  switch (environmentName) {
    case "prod":
      return prod;
    case "fairfax":
      return fairfax;
    default:
      throw new Error(`Unsupported test environment: "${environmentName}".`);
  }
}
