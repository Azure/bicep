// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  AzureEnvironment,
  getAzureResourceManagerClientOptions,
  getAzureScopes,
} from "../azure-environment";
import { validateResourceGroupName } from "../azure-pickers";

const environment: AzureEnvironment = {
  activeDirectoryEndpointUrl: "https://login.example.com/",
  activeDirectoryResourceId: "https://audience.example.com/",
  isCustomCloud: false,
  managementEndpointUrl: "https://management.core.example.com",
  name: "ExampleCloud",
  portalUrl: "https://portal.example.com",
  resourceManagerEndpointUrl: "https://management.example.com/",
};

describe("getAzureScopes", () => {
  test("GetAzureScopes_WithDefaultResource_AddsSlashBeforeDefaultScope", () => {
    expect(getAzureScopes(environment)).toEqual(["https://management.core.example.com/.default"]);
  });

  test("GetAzureScopes_WithNormalizedScope_PreservesScope", () => {
    expect(getAzureScopes(environment, undefined, "https://storage.example.com/.default")).toEqual([
      "https://storage.example.com/.default",
    ]);
  });

  test("GetAzureScopes_WithTenant_AddsTenantScope", () => {
    expect(getAzureScopes(environment, "tenant-id")).toEqual([
      "https://management.core.example.com/.default",
      "VSCODE_TENANT:tenant-id",
    ]);
  });
});

describe("getAzureResourceManagerClientOptions", () => {
  test("GetAzureResourceManagerClientOptions_WithSovereignEnvironment_UsesEnvironmentAudience", () => {
    const options = getAzureResourceManagerClientOptions(environment);

    expect(options.endpoint).toBe("https://management.example.com/");
    expect(options.credentials.scopes).toEqual(["https://audience.example.com/.default"]);
  });
});

describe("validateResourceGroupName", () => {
  test("ValidateResourceGroupName_WithValidUnicodeName_ReturnsUndefined", () => {
    expect(validateResourceGroupName("  r\u00e9gion_(1)-test  ")).toBeUndefined();
  });

  test("ValidateResourceGroupName_WithInvalidCharacter_ReturnsError", () => {
    expect(validateResourceGroupName("invalid/name")).toBeDefined();
  });

  test("ValidateResourceGroupName_WithTrailingPeriod_ReturnsError", () => {
    expect(validateResourceGroupName("invalid.")).toBeDefined();
  });

  test("ValidateResourceGroupName_WithMoreThanNinetyCharacters_ReturnsError", () => {
    expect(validateResourceGroupName("a".repeat(91))).toBeDefined();
  });
});
