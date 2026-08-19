// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzureEnvironment, getAzureResourceManagerClientOptions, getAzureScopes } from "../azure-environment";
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
  test("adds a slash before the default scope", () => {
    expect(getAzureScopes(environment)).toEqual(["https://management.core.example.com/.default"]);
  });

  test("preserves an already normalized scope", () => {
    expect(getAzureScopes(environment, undefined, "https://storage.example.com/.default")).toEqual([
      "https://storage.example.com/.default",
    ]);
  });

  test("adds the tenant scope", () => {
    expect(getAzureScopes(environment, "tenant-id")).toEqual([
      "https://management.core.example.com/.default",
      "VSCODE_TENANT:tenant-id",
    ]);
  });
});

describe("getAzureResourceManagerClientOptions", () => {
  test("uses the environment audience for a sovereign cloud", () => {
    const options = getAzureResourceManagerClientOptions(environment);

    expect(options.endpoint).toBe("https://management.example.com/");
    expect(options.credentials.scopes).toEqual(["https://audience.example.com/.default"]);
  });
});

describe("validateResourceGroupName", () => {
  test("accepts a valid Unicode name", () => {
    expect(validateResourceGroupName("  r\u00e9gion_(1)-test  ")).toBeUndefined();
  });

  test("rejects an invalid character", () => {
    expect(validateResourceGroupName("invalid/name")).toBeDefined();
  });

  test("rejects a trailing period", () => {
    expect(validateResourceGroupName("invalid.")).toBeDefined();
  });

  test("rejects names longer than 90 characters", () => {
    expect(validateResourceGroupName("a".repeat(91))).toBeDefined();
  });
});
