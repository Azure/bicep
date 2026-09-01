// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { workspace } from "vscode";

const configurationSection = "microsoft-sovereign-cloud";

export interface AzureEnvironment {
  readonly activeDirectoryEndpointUrl: string;
  readonly activeDirectoryResourceId: string;
  readonly isCustomCloud: boolean;
  readonly managementEndpointUrl: string;
  readonly name: string;
  readonly portalUrl: string;
  readonly resourceManagerEndpointUrl: string;
}

type AzureEnvironmentProfile = Omit<AzureEnvironment, "isCustomCloud">;

const azureCloud = {
  activeDirectoryEndpointUrl: "https://login.microsoftonline.com/",
  activeDirectoryResourceId: "https://management.core.windows.net/",
  managementEndpointUrl: "https://management.core.windows.net",
  name: "AzureCloud",
  portalUrl: "https://portal.azure.com",
  resourceManagerEndpointUrl: "https://management.azure.com/",
} satisfies AzureEnvironmentProfile;

const chinaCloud = {
  activeDirectoryEndpointUrl: "https://login.chinacloudapi.cn/",
  activeDirectoryResourceId: "https://management.core.chinacloudapi.cn/",
  managementEndpointUrl: "https://management.core.chinacloudapi.cn",
  name: "AzureChinaCloud",
  portalUrl: "https://portal.azure.cn",
  resourceManagerEndpointUrl: "https://management.chinacloudapi.cn",
} satisfies AzureEnvironmentProfile;

const usGovernment = {
  activeDirectoryEndpointUrl: "https://login.microsoftonline.us/",
  activeDirectoryResourceId: "https://management.core.usgovcloudapi.net/",
  managementEndpointUrl: "https://management.core.usgovcloudapi.net",
  name: "AzureUSGovernment",
  portalUrl: "https://portal.azure.us",
  resourceManagerEndpointUrl: "https://management.usgovcloudapi.net",
} satisfies AzureEnvironmentProfile;

export const knownAzureResourceManagerEndpoints = [
  azureCloud.resourceManagerEndpointUrl,
  chinaCloud.resourceManagerEndpointUrl,
  usGovernment.resourceManagerEndpointUrl,
] as const;

export function getConfiguredAzureEnvironment(): AzureEnvironment {
  const configuration = workspace.getConfiguration(configurationSection);
  const environment = configuration.get<string>("environment");

  switch (environment) {
    case "ChinaCloud":
      return { ...chinaCloud, isCustomCloud: false };
    case "USGovernment":
      return { ...usGovernment, isCustomCloud: false };
    case "custom": {
      const customEnvironment = configuration.get<unknown>("customEnvironment");
      if (!customEnvironment) {
        throw new Error("The custom Microsoft sovereign cloud environment is not configured.");
      }

      return { ...parseCustomEnvironment(customEnvironment), isCustomCloud: true };
    }
    default:
      return { ...azureCloud, isCustomCloud: false };
  }
}

export function getAzureAuthenticationProviderId(environment: AzureEnvironment): string {
  return environment.name === azureCloud.name ? "microsoft" : "microsoft-sovereign-cloud";
}

export function getAzureScopes(environment: AzureEnvironment, tenantId?: string, scopes?: string | string[]): string[] {
  const requestedScopes =
    scopes === undefined || scopes.length === 0 ? [environment.managementEndpointUrl] : [scopes].flat();
  const normalizedScopes = requestedScopes.map((scope) => {
    if (scope.endsWith(".default")) {
      return scope;
    }

    return `${scope.endsWith("/") ? scope : `${scope}/`}.default`;
  });

  if (tenantId) {
    normalizedScopes.push(`VSCODE_TENANT:${tenantId}`);
  }

  return [...new Set(normalizedScopes)];
}

export function getAzureResourceManagerClientOptions(environment: AzureEnvironment) {
  return {
    credentials: {
      scopes: getAzureScopes(environment, undefined, environment.activeDirectoryResourceId),
    },
    endpoint: environment.resourceManagerEndpointUrl,
  };
}

function parseCustomEnvironment(value: unknown): AzureEnvironmentProfile {
  if (!value || typeof value !== "object") {
    throw new Error("The custom Microsoft sovereign cloud environment must be an object.");
  }

  const properties = value as Record<string, unknown>;
  return {
    activeDirectoryEndpointUrl: getRequiredString(properties, "activeDirectoryEndpointUrl"),
    activeDirectoryResourceId: getRequiredString(properties, "activeDirectoryResourceId"),
    managementEndpointUrl: getRequiredString(properties, "managementEndpointUrl"),
    name: getRequiredString(properties, "name"),
    portalUrl: getRequiredString(properties, "portalUrl"),
    resourceManagerEndpointUrl: getRequiredString(properties, "resourceManagerEndpointUrl"),
  };
}

function getRequiredString(properties: Record<string, unknown>, propertyName: keyof AzureEnvironmentProfile): string {
  const value = properties[propertyName];
  if (typeof value !== "string" || value.length === 0) {
    throw new Error(`The custom Microsoft sovereign cloud environment requires a non-empty '${propertyName}'.`);
  }

  return value;
}
