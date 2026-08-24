// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { buildResourceTypeCatalog } from "../resource-palette";

describe("buildResourceTypeCatalog", () => {
  it("groups types and keeps the first API version for each type", () => {
    expect(
      buildResourceTypeCatalog([
        {
          fullyQualifiedType: "Microsoft.Storage/storageAccounts",
          apiVersion: "2025-01-01",
          isPreview: false,
        },
        {
          fullyQualifiedType: "Microsoft.Storage/storageAccounts",
          apiVersion: "2024-01-01",
          isPreview: false,
        },
        {
          fullyQualifiedType: "Microsoft.Network/virtualNetworks",
          apiVersion: "2024-07-01",
          isPreview: false,
        },
      ]),
    ).toEqual([
      {
        group: "Microsoft.Network",
        resourceTypes: [{ resourceType: "virtualNetworks", apiVersion: "2024-07-01" }],
      },
      {
        group: "Microsoft.Storage",
        resourceTypes: [{ resourceType: "storageAccounts", apiVersion: "2025-01-01" }],
      },
    ]);
  });
});
