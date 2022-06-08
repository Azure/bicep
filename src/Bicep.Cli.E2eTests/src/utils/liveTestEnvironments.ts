// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { EnvironmentOverrides } from "./types";

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
  registryUri: "biceptestff.azurecr.us",
  templateSpecSubscriptionId: "e21305d9-eef2-4990-8ed2-e2748236bee3",
  resourceSuffix: "ff",
  environmentOverrides: {
    AZURE_TENANT_ID: "63296244-ce2c-46d8-bc36-3e558792fbee",
    AZURE_CLIENT_ID: "fbbb49e5-e59c-4fce-9d7b-13925a36ad7f",
    AZURE_CLIENT_SECRET: process.env["BICEP_SPN_PASSWORD_FF"] ?? "",
  },
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
