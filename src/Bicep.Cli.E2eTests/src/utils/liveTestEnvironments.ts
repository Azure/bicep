// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { EnvironmentOverrides } from "./types";

export const prod = {
  suffix: ".prod",
  registryUri: "biceptestdf.azurecr.io",
  templateSpecSubscriptionId: "61e0a28a-63ed-4afc-9827-2ed09b7b30f3",
  resourceSuffix: "df",
} as const;

export const fairfax = {
  suffix: ".ff",
  registryUri: "biceptestff.azurecr.us",
  tenantId: "63296244-ce2c-46d8-bc36-3e558792fbee",
  clientId: "fbbb49e5-e59c-4fce-9d7b-13925a36ad7f",
  clientSecretEnvVar: "BICEP_SPN_PASSWORD_FF",
  templateSpecSubscriptionId: "e21305d9-eef2-4990-8ed2-e2748236bee3",
  resourceSuffix: "ff",
} as const;

export const fairfaxEnvironmentOverrides: EnvironmentOverrides = {
  AZURE_TENANT_ID: fairfax.tenantId,
  AZURE_CLIENT_ID: fairfax.clientId,
  AZURE_CLIENT_SECRET: process.env[fairfax.clientSecretEnvVar] ?? "",
};
